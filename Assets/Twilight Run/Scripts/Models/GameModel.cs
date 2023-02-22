using System;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics;
using System.Linq;

namespace Avangardum.TwilightRun.Models
{
    public class GameModel : IGameModel
    {
        private static readonly Random Random = new();
        
        private IGameConfig _gameConfig;
        private ISaver _saver;
        
        private int _whiteCharacterVerticalDirection;
        private float _worldForwardEdgeXPosition;
        private Vector2 _whiteCharacterPosition;
        private Vector2 _blackCharacterPosition;
        private List<Obstacle> _obstacles = new();
        private float _characterHorizontalSpeed;
        private float _characterVerticalSpeed;
        private float _characterVerticalAcceleration;
        private List<ObstacleGroup> _unfoldedObstacleGroups;

        public GameModel(IGameConfig gameConfig, ISaver saver)
        {
            _gameConfig = gameConfig;
            _saver = saver;
            
            _characterVerticalAcceleration = _gameConfig.CharacterHorizontalAcceleration * 
                (_gameConfig.CharacterBaseVerticalSpeed / _gameConfig.CharacterBaseHorizontalSpeed);

            _unfoldedObstacleGroups = _gameConfig.ObstacleGroups.SelectMany(UnfoldObstacleGroup).ToList();
        }
        
        public event EventHandler StateUpdated;
        public event EventHandler<ObstacleSpawnedEventArgs> ObstacleSpawned;
        public event EventHandler<ObstacleRemovedEventArgs> ObstacleRemoved;
        public event EventHandler GameOver;

        public Vector2 WhiteCharacterPosition => _whiteCharacterPosition;
        public Vector2 BlackCharacterPosition => _blackCharacterPosition;
        public IReadOnlyList<Obstacle> Obstacles => _obstacles;
        public bool IsGameOver { get; private set; } = true;
        public int Score => (int)(_whiteCharacterPosition.X * _gameConfig.ScorePerMeter);
        public bool IsPaused { get; set; } // TODO add pause

        public void Update(float deltaTime)
        {
            if (deltaTime == 0) return;
            if (deltaTime is < 0 or float.PositiveInfinity or float.NaN)
                throw new ArgumentOutOfRangeException(nameof(deltaTime), deltaTime, $"Invalid {nameof(deltaTime)} value.");
            
            if (IsGameOver || IsPaused) return;
            
            // If deltaTime is more than maxDeltaTime, unfold this update into several smaller updates.
            const float maxDeltaTime = 0.1f;
            while (deltaTime > maxDeltaTime)
            {
                Update(maxDeltaTime);
                deltaTime -= maxDeltaTime;
            }

            GenerateWorld();
            ProcessMovement();
            CheckCollisions();
            UpdateHighScore();
            CleanupWorld();

            StateUpdated?.Invoke(this, EventArgs.Empty);

            void ProcessMovement()
            {
                _characterHorizontalSpeed += _gameConfig.CharacterHorizontalAcceleration * deltaTime;
                _characterHorizontalSpeed = Math.Min(_characterHorizontalSpeed, _gameConfig.CharacterMaxHorizontalSpeed);
                _characterVerticalSpeed += _characterVerticalAcceleration * deltaTime;
                
                var horizontalCharacterMovement = _characterHorizontalSpeed * deltaTime;
                var verticalWhiteCharacterMovement = _whiteCharacterVerticalDirection * (_characterVerticalSpeed * deltaTime);
                var verticalBlackCharacterMovement = -verticalWhiteCharacterMovement;
                var whiteCharacterMovement = new Vector2(horizontalCharacterMovement, verticalWhiteCharacterMovement);
                var blackCharacterMovement = new Vector2(horizontalCharacterMovement, verticalBlackCharacterMovement);
                _whiteCharacterPosition += whiteCharacterMovement;
                _blackCharacterPosition += blackCharacterMovement;
                if (_whiteCharacterPosition.Y >= _gameConfig.MaxCharacterYPosition)
                {
                    _whiteCharacterVerticalDirection = 0;
                    _whiteCharacterPosition.Y = _gameConfig.MaxCharacterYPosition;
                    _blackCharacterPosition.Y = _gameConfig.MinCharacterYPosition;
                }
                else if (_whiteCharacterPosition.Y <= _gameConfig.MinCharacterYPosition)
                {
                    _whiteCharacterVerticalDirection = 0;
                    _whiteCharacterPosition.Y = _gameConfig.MinCharacterYPosition;
                    _blackCharacterPosition.Y = _gameConfig.MaxCharacterYPosition;
                }
            }

            void GenerateWorld()
            {
                while (_whiteCharacterPosition.X + _gameConfig.WorldGenerationZoneForwardSize > _worldForwardEdgeXPosition)
                {
                    if (_gameConfig.ObstacleGroups.Count == 0) throw new InvalidOperationException("No obstacle groups.");
                    var availableObstacleGroups = _unfoldedObstacleGroups
                        .Where(og => og.Difficulty <= Score)
                        .ToList();
                    var obstacleGroup = availableObstacleGroups[Random.Next(availableObstacleGroups.Count)];
                    SpawnObstacleGroup(obstacleGroup);
                }
            }
            
            void CleanupWorld()
            {
                var worldGenerationZoneBackEdgeX = _whiteCharacterPosition.X - _gameConfig.WorldGenerationZoneBackSize;
                while (IsCleanupNeeded())
                {
                    RemoveObstacle(_obstacles[0]);
                }

                bool IsCleanupNeeded() => _obstacles.Count > 0 && _obstacles[0].Position.X < worldGenerationZoneBackEdgeX;
            }

            void CheckCollisions()
            {
                var characterPositions = new List<Vector2> { WhiteCharacterPosition, BlackCharacterPosition };
                var characterSizes = new List<Vector2> { _gameConfig.CharacterSize, _gameConfig.CharacterSize };
                var characterColors = new List<GameColor> { GameColor.White, GameColor.Black };
                var characterData = characterPositions
                    .Zip(characterSizes, (position, size) => (position, size))
                    .Zip(characterColors, (positionAndSize, color) => (positionAndSize.position, positionAndSize.size, color) )
                    .ToList();
                foreach (var (characterPosition, characterSize, characterColor) in characterData)
                {
                    foreach (var obstacle in _obstacles)
                    {
                        var obstaclePosition = obstacle.Position;
                        var obstacleSize = obstacle.Size;
                        var obstacleColor = obstacle.Color;
                        
                        var areColliding = 
                            AabbUtil.DoAabbsIntersect(characterPosition, characterSize, obstaclePosition, obstacleSize) &&
                            characterColor != obstacleColor;
                        if (areColliding)
                        {
                            SetGameOver();
                            return;
                        }
                    }
                }
            }
            
            void UpdateHighScore()
            {
                if (Score > _saver.HighScore)
                {
                    _saver.HighScore = Score;
                }
            }
        }

        public void Swap()
        {
            if (_whiteCharacterVerticalDirection != 0) return;
            Debug.Assert(new[] { _gameConfig.MinCharacterYPosition, _gameConfig.MaxCharacterYPosition }.
                Contains(_whiteCharacterPosition.Y));
            _whiteCharacterVerticalDirection = _whiteCharacterPosition.Y == _gameConfig.MinCharacterYPosition ? 1 : -1;
        }
        
        public void Restart()
        {
            _whiteCharacterPosition = new(0, _gameConfig.MaxCharacterYPosition);
            _blackCharacterPosition = new(0, _gameConfig.MinCharacterYPosition);
            _worldForwardEdgeXPosition = _gameConfig.StartSafeZoneSize;
            _characterHorizontalSpeed = _gameConfig.CharacterBaseHorizontalSpeed;
            _characterVerticalSpeed = _gameConfig.CharacterBaseVerticalSpeed;
            IsGameOver = false;
            _obstacles.ToList().ForEach(RemoveObstacle);

            StateUpdated?.Invoke(this, EventArgs.Empty);
        }


        private void SpawnObstacleGroup(ObstacleGroup obstacleGroup)
        {
            foreach (var obstacle in obstacleGroup.Obstacles)
            {
                SpawnObstacle(obstacle.Position + new Vector2(_worldForwardEdgeXPosition, 0), obstacle.Size, obstacle.Color);
            }
            _worldForwardEdgeXPosition += obstacleGroup.Size;
        }

        private void SpawnObstacle(Vector2 position, Vector2 size, GameColor color)
        {
            var obstacle = new Obstacle(position, size, color);
            _obstacles.Add(obstacle);
            ObstacleSpawned?.Invoke(this, new ObstacleSpawnedEventArgs(obstacle));
        }

        private void RemoveObstacle(Obstacle obstacle)
        {
            _obstacles.Remove(obstacle);
            ObstacleRemoved?.Invoke(this, new ObstacleRemovedEventArgs(obstacle.Id));
        }
        
        private void SetGameOver()
        {
            IsGameOver = true;
            GameOver?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Converts an obstacle group to a list of obstacle groups with weight of 1.
        /// </summary>
        private List<ObstacleGroup> UnfoldObstacleGroup(ObstacleGroup inputGroup)
        {
            var outputGroup = new ObstacleGroup(inputGroup.Obstacles, inputGroup.Size, 1, inputGroup.Difficulty);
            return Enumerable.Repeat(outputGroup, inputGroup.Weight).ToList();
        }
    }
}