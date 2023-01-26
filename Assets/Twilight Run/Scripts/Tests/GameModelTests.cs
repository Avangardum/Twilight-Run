using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Avangardum.TwilightRun.Main;
using NUnit.Framework;
using Zenject;
using Avangardum.TwilightRun.Models;
using UnityEditor;

namespace Avangardum.TwilightRun.Tests
{
    public class GameModelTests : ZenjectUnitTestFixture
    {
        private IGameModel _gameModel;
        private TestGameConfig _testGameConfig;
        
        [Inject]
        public void Inject(IGameModel gameModel, TestGameConfig testGameConfig)
        {
            _gameModel = gameModel;
            _testGameConfig = testGameConfig;
        }
        
        [SetUp]
        public void CSetUp()
        {
            Container.Bind<IGameModel>().To<GameModel>().AsSingle();
            const string gameConfigPath = "Assets/Twilight Run/Game Config.asset";
            var gameConfig = AssetDatabase.LoadAssetAtPath<GameConfig>(gameConfigPath);
            var testGameConfig = new TestGameConfig(gameConfig);
            Container.BindInterfacesAndSelfTo<TestGameConfig>().FromInstance(testGameConfig).AsSingle();
            
            Container.Inject(this);
        }
        
        [Test]
        public void CharactersMoveForward()
        {
            Assert.That(_gameModel.WhiteCharacterPosition.X, Is.EqualTo(_gameModel.BlackCharacterPosition.X));
            var whiteCharacterPosition = _gameModel.WhiteCharacterPosition;
            var blackCharacterPosition = _gameModel.BlackCharacterPosition;
            _gameModel.Update(0.01f);
            Assert.That(_gameModel.WhiteCharacterPosition.X, Is.GreaterThan(whiteCharacterPosition.X));
            Assert.That(_gameModel.BlackCharacterPosition.X, Is.GreaterThan(blackCharacterPosition.X));
            Assert.That(_gameModel.WhiteCharacterPosition.X, Is.EqualTo(_gameModel.BlackCharacterPosition.X));
        }

        [Test]
        public void CharactersDontChangeYPositionIfSwapNotCalled()
        {
            var whiteCharacterYPosition = _gameModel.WhiteCharacterPosition.Y;
            var blackCharacterYPosition = _gameModel.BlackCharacterPosition.Y;
            _gameModel.Update(0.01f);
            Assert.That(_gameModel.WhiteCharacterPosition.Y, Is.EqualTo(whiteCharacterYPosition));
            Assert.That(_gameModel.BlackCharacterPosition.Y, Is.EqualTo(blackCharacterYPosition));
        }

        [Test]
        public void StateUpdatedInvokedAfterUpdateWithNonZeroDeltaTime()
        {
            _gameModel.StateUpdated += (s, e) => Assert.Pass();
            _gameModel.Update(0.01f);
            Assert.Fail();
        }
        
        [Test]
        public void StateUpdatedNotInvokedAfterUpdateWithZeroDeltaTime()
        {
            _gameModel.StateUpdated += (s, e) => Assert.Fail();
            _gameModel.Update(0);
            Assert.Pass();
        }
        
        [Test]
        public void UpdateCalledWithInvalidDeltaTimeThrowsArgumentException(
            [Values(-0.01f, -0.3f, -2f, float.NegativeInfinity, float.PositiveInfinity, float.NaN)] float deltaTime)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _gameModel.Update(deltaTime));
        }

        [Test]
        public void SwapCharactersChangesCharactersPlaces()
        {
            var whiteCharacterYPosition = _gameModel.WhiteCharacterPosition.Y;
            var blackCharacterYPosition = _gameModel.BlackCharacterPosition.Y;
            _gameModel.Swap();
            Wait(3);
            Assert.That(_gameModel.WhiteCharacterPosition.Y, Is.EqualTo(blackCharacterYPosition));
            Assert.That(_gameModel.BlackCharacterPosition.Y, Is.EqualTo(whiteCharacterYPosition));
        }
        
        [Test]
        public void SwapCharactersDoesntDoAnythingWhenCalledInSecondTimeShortlyAfterFirst()
        {
            var whiteCharacterYPosition = _gameModel.WhiteCharacterPosition.Y;
            var blackCharacterYPosition = _gameModel.BlackCharacterPosition.Y;
            _gameModel.Swap();
            Wait(0.1f);
            _gameModel.Swap();
            Wait(2.9f);
            Assert.That(_gameModel.WhiteCharacterPosition.Y, Is.EqualTo(blackCharacterYPosition));
            Assert.That(_gameModel.BlackCharacterPosition.Y, Is.EqualTo(whiteCharacterYPosition));
        }

        [Test]
        public void ObstaclesSpawnButNotInSafeZone()
        {
            bool wasObstacleSpawned = false;
            _gameModel.ObstacleSpawned += (_, _) => wasObstacleSpawned = true;
            Wait(0.01f);
            Assert.That(_gameModel.Obstacles, Is.Not.Empty);
            Assert.That(wasObstacleSpawned, Is.True);
            var firstObstacle = _gameModel.Obstacles.OrderBy(o => o.Position.X).First();
            Assert.That(firstObstacle.Position.X, Is.GreaterThanOrEqualTo(_testGameConfig.StartSafeZoneSize));
        }

        [TestCase(0.05f, 1)]
        [TestCase(0.11f, 2)]
        [TestCase(0.43f, 5)]
        public void UpdateWithTooBigDeltaTimeUnfoldsIntoMultipleUpdates(float deltaTime, int expectedUpdateCount)
        {
            int updateCount = 0;
            _gameModel.StateUpdated += (_, _) => updateCount++;
            _gameModel.Update(deltaTime);
            Assert.That(updateCount, Is.EqualTo(expectedUpdateCount));
        }

        [Test]
        public void ObstaclesFromConfigCreated()
        {
            _testGameConfig.ObstacleGroups.Clear();
            var testObstacleGroup = new ObstacleGroup(new List<Obstacle>
            {
                new(Vector2.Zero, Vector2.One, GameColor.Red),
                new(new Vector2(5, 0), Vector2.One, GameColor.Black),
                new(new Vector2(10, 0), Vector2.One, GameColor.White),
            }, 15);
            _testGameConfig.ObstacleGroups.Add(testObstacleGroup);
            
            _gameModel.Update(0.01f);
            var obstacles = _gameModel.Obstacles;
            var safeZoneSize = _testGameConfig.StartSafeZoneSize;
            Assert.That(obstacles, Has.Count.GreaterThanOrEqualTo(3));
            Assert.That(obstacles[0].Position, Is.EqualTo(new Vector2(safeZoneSize, 0)));
            Assert.That(obstacles[1].Position, Is.EqualTo(new Vector2(safeZoneSize + 5, 0)));
            Assert.That(obstacles[2].Position, Is.EqualTo(new Vector2(safeZoneSize + 10, 0)));
        }

        [Test]
        public void ObstaclesBehindWorldGenerationZoneRemoved()
        {
            SetHarmlessObstacleGroupConfig();
            bool wasObstacleRemoved = false;
            _gameModel.ObstacleRemoved += (_, _) => wasObstacleRemoved = true;
            for (int i = 0; i < 10; i++)
            {
                Wait(5);
                var obstacleXPositions = _gameModel.Obstacles.Select(o => o.Position.X);
                var minObstacleXPosition = _gameModel.WhiteCharacterPosition.X - _testGameConfig.WorldGenerationZoneBackSize;
                Assert.That(obstacleXPositions, Has.All.GreaterThanOrEqualTo(minObstacleXPosition));
            }
            Assert.That(wasObstacleRemoved, Is.True);
        }
        
        [Test]
        public void InactivityWithHarmlessObstaclesDoesntEndGameAndDoesntStopCharacters()
        {
            SetHarmlessObstacleGroupConfig();
            Wait(10);
            Assert.That(_gameModel.IsGameOver, Is.False);
            var whiteCharacterXPosition = _gameModel.WhiteCharacterPosition.X;
            var blackCharacterXPosition = _gameModel.BlackCharacterPosition.X;
            Wait(1);
            Assert.That(_gameModel.WhiteCharacterPosition.X, Is.Not.EqualTo(whiteCharacterXPosition));
            Assert.That(_gameModel.BlackCharacterPosition.X, Is.Not.EqualTo(blackCharacterXPosition));
        }
        
        [Test]
        public void InactivityWithHarmfulObstaclesEndsGameAndStopsCharacters()
        {
            SetHarmfulObstacleGroupConfig();
            Wait(10);
            Assert.That(_gameModel.IsGameOver, Is.True);
            var whiteCharacterXPosition = _gameModel.WhiteCharacterPosition.X;
            var blackCharacterXPosition = _gameModel.BlackCharacterPosition.X;
            Wait(1);
            Assert.That(_gameModel.WhiteCharacterPosition.X, Is.EqualTo(whiteCharacterXPosition));
            Assert.That(_gameModel.BlackCharacterPosition.X, Is.EqualTo(blackCharacterXPosition));
        }
        
        private void Wait(float time, float timeStep = 0.02f)
        {
            var timeLeft = time;
            while (timeLeft >= timeStep)
            {
                _gameModel.Update(timeStep);
                timeLeft -= timeStep;
            }
            _gameModel.Update(timeLeft);
        }

        private void SetHarmlessObstacleGroupConfig()
        {
            _testGameConfig.ObstacleGroups.Clear();
            _testGameConfig.ObstacleGroups.Add(new ObstacleGroup(new List<Obstacle>
            {
                new(new Vector2(0, 0.5f), Vector2.One, GameColor.Black),
                new(new Vector2(0, 14.5f), Vector2.One, GameColor.White),
            }, 5));
        }
        
        private void SetHarmfulObstacleGroupConfig()
        {
            _testGameConfig.ObstacleGroups.Clear();
            _testGameConfig.ObstacleGroups.Add(new ObstacleGroup(new List<Obstacle>
            {
                new(new Vector2(0, 0.5f), Vector2.One, GameColor.Red),
                new(new Vector2(0, 14.5f), Vector2.One, GameColor.Red),
            }, 5));
        }
    }
}