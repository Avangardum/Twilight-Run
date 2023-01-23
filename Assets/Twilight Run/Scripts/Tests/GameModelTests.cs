using System;
using NUnit.Framework;
using Zenject;
using Avangardum.TwilightRun.Models;

namespace Avangardum.TwilightRun.Tests
{
    public class GameModelTests : ZenjectUnitTestFixture
    {
        private IGameModel _gameModel;
        
        [Inject]
        public void Inject(IGameModel gameModel)
        {
            _gameModel = gameModel;
        }
        
        public override void Setup()
        {
            base.Setup();

            Container.Bind<IGameModel>().To<GameModel>().AsSingle();
            
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