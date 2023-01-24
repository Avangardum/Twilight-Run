using System;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics;
using System.Linq;

namespace Avangardum.TwilightRun.Models
{
    public class GameModel : IGameModel
    {
        private IGameConfig _gameConfig;
        
        private int _whiteCharacterVerticalDirection;
        private float _worldForwardEdgeXPosition;
        private Vector2 _whiteCharacterPosition;
        private Vector2 _blackCharacterPosition;
        private List<Obstacle> _obstacles = new();

        public GameModel(IGameConfig gameConfig)
        {
            _gameConfig = gameConfig;
            _whiteCharacterPosition = new(0, _gameConfig.MaxCharacterYPosition);
            _blackCharacterPosition = new(0, _gameConfig.MinCharacterYPosition);
            _worldForwardEdgeXPosition = _gameConfig.StartSafeZoneSize;
        }

        public event EventHandler StateUpdated;

        public Vector2 WhiteCharacterPosition => _whiteCharacterPosition;

        public Vector2 BlackCharacterPosition => _blackCharacterPosition;
        public IReadOnlyList<Obstacle> Obstacles => _obstacles;

        public void Update(float deltaTime)
        {
            if (deltaTime == 0) return;
            if (deltaTime is < 0 or float.PositiveInfinity or float.NaN)
                throw new ArgumentOutOfRangeException(nameof(deltaTime), deltaTime, $"Invalid {nameof(deltaTime)} value.");

            GenerateWorld();
            ProcessMovement();

            StateUpdated?.Invoke(this, EventArgs.Empty);

            void ProcessMovement()
            {
                var horizontalCharacterMovement = _gameConfig.CharacterHorizontalSpeed * deltaTime;
                var verticalWhiteCharacterMovement = _whiteCharacterVerticalDirection * (_gameConfig.CharacterVerticalSpeed * deltaTime);
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
                    _obstacles.Add(new Obstacle(new Vector2(_worldForwardEdgeXPosition, _gameConfig.MinCharacterYPosition), 
                        Vector2.One, GameColor.Red));
                    _worldForwardEdgeXPosition += 10;
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
    }
}