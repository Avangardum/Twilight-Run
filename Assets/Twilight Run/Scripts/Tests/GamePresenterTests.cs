using System;
using System.Collections.Generic;
using Zenject;
using Avangardum.TwilightRun.Models;
using Avangardum.TwilightRun.Presenters;
using NUnit.Framework;
using UVector3 = UnityEngine.Vector3;
using SVector2 = System.Numerics.Vector2;

namespace Avangardum.TwilightRun.Tests
{
    public class GamePresenterTests : ZenjectUnitTestFixture
    {
        private class GameModelMock : IGameModel
        {
            public event EventHandler StateUpdated;

            public SVector2 WhiteCharacterPosition { get; set; }
            public SVector2 BlackCharacterPosition { get; set; }
            public IReadOnlyList<Obstacle> Obstacles { get; } = new List<Obstacle>();
            public bool WasSwapCalled { get; set; }
            
            public void Update(float deltaTime) => StateUpdated?.Invoke(this, EventArgs.Empty);

            public void Swap()
            {
                WasSwapCalled = true;
            }
        }
        
        private class GameViewMock : IGameView
        {
            public event EventHandler ScreenTapped;
            
            public UVector3 WhiteCharacterPosition { get; set; }
            public UVector3 BlackCharacterPosition { get; set; }
            
            public void InvokeScreenTapped() => ScreenTapped?.Invoke(this, EventArgs.Empty);
        }
        
        private GameModelMock _gameModel;
        private GameViewMock _gameView;

        [SetUp]
        public new void Setup()
        {
            Container.BindInterfacesAndSelfTo<GameModelMock>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameViewMock>().AsSingle();
            Container.Bind<GamePresenter>().AsSingle();

            Container.Resolve<GamePresenter>();
            Container.Inject(this);
        }

        [Inject]
        private void Inject(GameModelMock gameModel, GameViewMock gameView)
        {
            _gameModel = gameModel;
            _gameView = gameView;
        }

        private static readonly object[] UpdatesViewCharacterPositionsOnModelStateUpdatedCases =
        {
            new object[] { new SVector2(4, 4), new SVector2(4, -4), new UVector3(0, 4, 4), new UVector3(0, -4, 4) },
            new object[] { new SVector2(10, 15), new SVector2(-20, -25), new UVector3(0, 15, 10), new UVector3(0, -25, -20) }
        };
        
        [TestCaseSource(nameof(UpdatesViewCharacterPositionsOnModelStateUpdatedCases))]
        public void UpdatesViewCharacterPositionsOnModelStateUpdated(
            SVector2 modelWhiteCharacterPosition, SVector2 modelBlackCharacterPosition, 
            UVector3 viewWhiteCharacterPosition, UVector3 viewBlackCharacterPosition)
        {
            _gameModel.WhiteCharacterPosition = modelWhiteCharacterPosition;
            _gameModel.BlackCharacterPosition = modelBlackCharacterPosition;
            _gameModel.Update(0.01f);
            Assert.That(_gameView.WhiteCharacterPosition, Is.EqualTo(viewWhiteCharacterPosition));
            Assert.That(_gameView.BlackCharacterPosition, Is.EqualTo(viewBlackCharacterPosition));
        }

        [Test]
        public void ScreenTapCausesSwap()
        {
            Assert.That(_gameModel.WasSwapCalled, Is.False);
            _gameView.InvokeScreenTapped();
            Assert.That(_gameModel.WasSwapCalled, Is.True);
        }
    }
}