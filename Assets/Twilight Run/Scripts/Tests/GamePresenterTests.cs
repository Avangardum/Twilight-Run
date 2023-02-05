using System;
using System.Collections.Generic;
using Avangardum.TwilightRun.Main;
using Zenject;
using Avangardum.TwilightRun.Models;
using Avangardum.TwilightRun.Presenters;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using IGameConfig = Avangardum.TwilightRun.Presenters.IGameConfig;
using UVector3 = UnityEngine.Vector3;
using SVector2 = System.Numerics.Vector2;

namespace Avangardum.TwilightRun.Tests
{
    public class GamePresenterTests : ZenjectUnitTestFixture
    {
        private class GameModelMock : IGameModel
        {
            public event EventHandler StateUpdated;
            public event EventHandler<ObstacleSpawnedEventArgs> ObstacleSpawned;
            public event EventHandler<ObstacleRemovedEventArgs> ObstacleRemoved;
            
            public SVector2 WhiteCharacterPosition { get; set; }
            public SVector2 BlackCharacterPosition { get; set; }
            public IReadOnlyList<Obstacle> Obstacles { get; } = new List<Obstacle>();
            public bool IsGameOver { get; set; }
            public int Score { get; set; }
            public bool WasSwapCalled { get; set; }
            public bool WasRestartCalled { get; private set; }


            public void Update(float deltaTime) => StateUpdated?.Invoke(this, EventArgs.Empty);
            
            public void Swap()
            {
                WasSwapCalled = true;
            }

            public void Restart()
            {
                WasRestartCalled = true;
            }

            public void InvokeObstacleSpawned(Obstacle obstacle) => 
                ObstacleSpawned?.Invoke(this, new ObstacleSpawnedEventArgs(obstacle));

            public void InvokeObstacleRemoved(int id) =>
                ObstacleRemoved?.Invoke(this, new ObstacleRemovedEventArgs(id));
        }
        
        private class GameViewMock : IGameView
        {
            public event EventHandler ScreenTapped;
            public event EventHandler PlayButtonClicked;

            public UVector3 WhiteCharacterPosition { get; set; }
            public UVector3 BlackCharacterPosition { get; set; }
            public int Score { get; set; }
            public int HighScore { get; set; }
            public bool IsGameOver { get; set; }
            public bool HasRelevantGameState { get; set; }
            public UVector3? LastCreatedObstaclePosition { get; private set; }
            public UVector3? LastCreatedObstacleSize { get; private set; }
            public Color? LastCreatedObstacleColor { get; private set; }
            public int? LastRemovedObstacleId { get; private set; }
            
            public void CreateObstacleView(int id, UVector3 position, UVector3 size, Color color)
            {
                LastCreatedObstaclePosition = position;
                LastCreatedObstacleSize = size;
                LastCreatedObstacleColor = color;
            }

            public void RemoveObstacleView(int id)
            {
                LastRemovedObstacleId = id;
            }

            public void InvokeScreenTapped() => ScreenTapped?.Invoke(this, EventArgs.Empty);
            
            public void InvokePlayButtonClicked() => PlayButtonClicked?.Invoke(this, EventArgs.Empty);
        }
        
        private class SaverMock : Presenters.ISaver
        {
            private int _highScore;
            public event EventHandler<HighScoreChangedEventArgs> HighScoreChanged;

            public int HighScore
            {
                get => _highScore;
                set
                {
                    if (_highScore == value) return;
                    _highScore = value; 
                    HighScoreChanged?.Invoke(this, new HighScoreChangedEventArgs(value));
                }
            }
        }

        private GameModelMock _gameModel;
        private GameViewMock _gameView;
        private SaverMock _saver;

        [SetUp]
        public new void Setup()
        {
            Container.BindInterfacesAndSelfTo<GameModelMock>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameViewMock>().AsSingle();
            Container.Bind<GamePresenter>().AsSingle();
            Container.BindInterfacesAndSelfTo<SaverMock>().AsSingle();
            Container.Bind<IGameConfig>().FromInstance(TestUtil.GameConfig).AsSingle();

            Container.Resolve<GamePresenter>();
            Container.Inject(this);
        }

        [Inject]
        private void Inject(GameModelMock gameModel, GameViewMock gameView, SaverMock saver)
        {
            _gameModel = gameModel;
            _gameView = gameView;
            _saver = saver;
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
            Assume.That(_gameModel.WasSwapCalled, Is.False);
            _gameView.InvokeScreenTapped();
            Assert.That(_gameModel.WasSwapCalled, Is.True);
        }

        [Test] 
        public void ObstacleViewCreatedOnObstacleSpawned()
        {
            var obstacle = new Obstacle(SVector2.One, SVector2.One * 2, GameColor.Red);
            _gameModel.InvokeObstacleSpawned(obstacle);
            Assert.That(_gameView.LastCreatedObstaclePosition, Is.EqualTo(new UVector3(0, 1, 1)));
            Assert.That(_gameView.LastCreatedObstacleSize, Is.EqualTo(new UVector3(1, 2, 2)));
            Assert.That(_gameView.LastCreatedObstacleColor, Is.EqualTo(Color.red));
        }
        
        [Test]
        public void ObstacleViewRemovedOnObstacleRemoved()
        {
            _gameModel.InvokeObstacleRemoved(42);
            Assert.That(_gameView.LastRemovedObstacleId, Is.EqualTo(42));
        }
        
        [Test]
        public void UpdatesViewScoreOnModelStateUpdated()
        {
            _gameModel.Score = 42;
            _gameModel.Update(0.01f);
            Assert.That(_gameView.Score, Is.EqualTo(42));
        }
        
        [Test]
        public void GameOverSetOnViewOnGameOver()
        {
            _gameModel.IsGameOver = true;
            _gameModel.Update(0.01f);
            Assert.That(_gameView.IsGameOver, Is.True);
        }

        [Test]
        public void GameRestartedOnPlayButtonClicked()
        {
            Assume.That(_gameModel.WasRestartCalled, Is.False);
            _gameView.InvokePlayButtonClicked();
            Assert.That(_gameModel.WasRestartCalled, Is.True);
        }
        
        [Test]
        public void HighScoreSetOnViewOnHighScoreChanged()
        {
            Assume.That(_gameView.HighScore, Is.EqualTo(0));
            _saver.HighScore = 42;
            Assert.That(_gameView.HighScore, Is.EqualTo(42));
        }

        [Test]
        public void GameViewHasRelevantGameStateAfterFirstUpdate()
        {
            Assume.That(_gameView.HasRelevantGameState, Is.False);
            _gameModel.Update(0.02f);
            Assert.That(_gameView.HasRelevantGameState, Is.True);
        }
    }
}