using System;
using System.Linq;
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
        private IGameConfig _gameConfig;
        
        [Inject]
        public void Inject(IGameModel gameModel, IGameConfig gameConfig)
        {
            _gameModel = gameModel;
            _gameConfig = gameConfig;
        }
        
        [SetUp]
        public void CSetUp()
        {
            Container.Bind<IGameModel>().To<GameModel>().AsSingle();
            const string gameConfigPath = "Assets/Twilight Run/Game Config.asset";
            var gameConfig = AssetDatabase.LoadAssetAtPath<GameConfig>(gameConfigPath);
            Container.Bind<IGameConfig>().FromInstance(gameConfig).AsSingle();
            
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
            Assert.That(firstObstacle.Position.X, Is.GreaterThanOrEqualTo(_gameConfig.StartSafeZoneSize));
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
    }
}