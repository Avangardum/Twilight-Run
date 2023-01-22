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
    }
}