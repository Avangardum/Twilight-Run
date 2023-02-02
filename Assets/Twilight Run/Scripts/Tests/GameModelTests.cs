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
        private class MockSaver : ISaver
        {
            public int HighScore { get; set; }
        }

        private IGameModel _gameModel;
        private TestGameConfig _testGameConfig;
        private MockSaver _saver;
        
        [Inject]
        private void Inject(IGameModel gameModel, TestGameConfig testGameConfig, MockSaver saver)
        {
            _gameModel = gameModel;
            _testGameConfig = testGameConfig;
            _saver = saver;
        }
        
        [SetUp]
        public void CSetUp()
        {
            Container.Bind<IGameModel>().To<GameModel>().AsTransient();
            const string gameConfigPath = "Assets/Twilight Run/Game Config.asset";
            IGameConfig gameConfig = AssetDatabase.LoadAssetAtPath<GameConfig>(gameConfigPath);
            var testGameConfig = new TestGameConfig(gameConfig);
            Container.BindInterfacesAndSelfTo<TestGameConfig>().FromInstance(testGameConfig).AsSingle();
            Container.BindInterfacesAndSelfTo<MockSaver>().AsSingle();
            
            Container.Inject(this);
            
            _gameModel.Restart();
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

        [Test]
        public void CharactersAlwaysHaveSameXPosition()
        {
            for (int i = 0; i < 1000; i++)
            {
                Assert.That(_gameModel.WhiteCharacterPosition.X, Is.EqualTo(_gameModel.BlackCharacterPosition.X));
                _gameModel.Update(0.02f);
            }
        }
        
        [Test]
        public void CharactersAccelerate()
        {
            SetHarmlessObstacleGroupConfig();
            var previousWhiteCharacterMovement = 0f;
            for (int i = 0; i < 60; i++)
            {
                var whiteCharacterXPosition = _gameModel.WhiteCharacterPosition.X;
                Wait(1);
                var whiteCharacterXMovement = _gameModel.WhiteCharacterPosition.X - whiteCharacterXPosition;
                Assert.That(whiteCharacterXMovement, Is.GreaterThan(previousWhiteCharacterMovement));
                previousWhiteCharacterMovement = whiteCharacterXMovement;
            }
        }

        [Test]
        public void CharactersStopAcceleratingWhenMaxSpeedIsReached()
        {
            SetHarmlessObstacleGroupConfig();
            Wait(600);
            float? previousWhiteCharacterMovement = null;
            for (int i = 0; i < 60; i++)
            {
                var whiteCharacterXPosition = _gameModel.WhiteCharacterPosition.X;
                Wait(1);
                var whiteCharacterXMovement = _gameModel.WhiteCharacterPosition.X - whiteCharacterXPosition;
                if (previousWhiteCharacterMovement != null)
                    Assert.That(whiteCharacterXMovement, Is.EqualTo(previousWhiteCharacterMovement));
                previousWhiteCharacterMovement = whiteCharacterXMovement;
            }
        }

        [Test]
        public void CharacterVerticalSpeedIncreases()
        {
            SetHarmlessObstacleGroupConfig();
            _gameModel.Swap();
            var swapTime1 = MeasureSwapTime();
            Wait(60);
            var swapTime2 = MeasureSwapTime();
            Assert.That(swapTime2, Is.LessThan(swapTime1)); 

            float MeasureSwapTime()
            {
                Assume.That(_gameModel.WhiteCharacterPosition.Y, Is.EqualTo(_testGameConfig.MaxCharacterYPosition)
                    .Or.EqualTo(_testGameConfig.MinCharacterYPosition));
                float timeElapsed = 0;
                _gameModel.Swap();
                do
                {
                    _gameModel.Update(0.02f);
                    timeElapsed += 0.02f;
                } while (_gameModel.WhiteCharacterPosition.Y != _testGameConfig.MinCharacterYPosition &&
                         _gameModel.WhiteCharacterPosition.Y != _testGameConfig.MaxCharacterYPosition);
                return timeElapsed;
            }
        }

        [Test]
        public void ScoreIncreasesAccordingToConfig()
        {
            var whiteCharacterXPosition = _gameModel.WhiteCharacterPosition.X;
            Wait(1);
            var whiteCharacterXMovement = _gameModel.WhiteCharacterPosition.X - whiteCharacterXPosition;
            var expectedScore = (int)(whiteCharacterXMovement * _testGameConfig.ScorePerMeter);
            Assert.That(_gameModel.Score, Is.EqualTo(expectedScore));
        }

        [Test]
        public void RestartResetsState()
        {
            SetHarmfulObstacleGroupConfig();
            Wait(10);
            var characterXPosition1 = _gameModel.WhiteCharacterPosition.X;
            var score1 = _gameModel.Score;
            var obstaclesCount1 = _gameModel.Obstacles.Count;
            _gameModel.Restart();
            Assert.That(_gameModel.WhiteCharacterPosition.X, Is.EqualTo(0));
            Assert.That(_gameModel.Score, Is.EqualTo(0));
            Wait(10);
            var characterXPosition2 = _gameModel.WhiteCharacterPosition.X;
            var score2 = _gameModel.Score;
            var obstaclesCount2 = _gameModel.Obstacles.Count;
            Assert.That(characterXPosition2, Is.EqualTo(characterXPosition1));
            Assert.That(score2, Is.EqualTo(score1));
            Assert.That(obstaclesCount2, Is.EqualTo(obstaclesCount1));
        }

        [Test]
        public void SavesHighScore()
        {
            SetHarmlessObstacleGroupConfig();
            const int initialHighScore = 10;
            _saver.HighScore = initialHighScore;
            while (_gameModel.Score <= initialHighScore)
            {
                Assert.That(_saver.HighScore, Is.EqualTo(initialHighScore));
                Wait(1);
            }
            for (int i = 0; i < 10; i++)
            {
                Assert.That(_saver.HighScore, Is.EqualTo(_gameModel.Score));
                Wait(1);
            }
        }

        [Test]
        public void DoesNotBreakWhenStartSafeZoneIsHuge()
        {
            _testGameConfig.StartSafeZoneSize = 1000;
            _gameModel.Restart();
            Wait(10);
        }

        [Test]
        public void DoesNotDoAnythingUntilRestartCalled()
        {
            _gameModel = Container.Resolve<IGameModel>();
            var whiteCharacterXPosition = _gameModel.WhiteCharacterPosition.X;
            Wait(1);
            Assert.That(_gameModel.WhiteCharacterPosition.X, Is.EqualTo(whiteCharacterXPosition));
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