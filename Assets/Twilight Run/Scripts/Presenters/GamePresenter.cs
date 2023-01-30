using System;
using System.Diagnostics.Contracts;
using Avangardum.TwilightRun.Models;
using UnityEngine;
using UVector3 = UnityEngine.Vector3;
using SVector2 = System.Numerics.Vector2;

namespace Avangardum.TwilightRun.Presenters
{
    public class GamePresenter
    {
        private IGameModel _gameModel;
        private IGameView _gameView;
        private ISaver _saver;

        public GamePresenter(IGameModel gameModel, IGameView gameView, ISaver saver)
        {
            _gameModel = gameModel;
            _gameView = gameView;
            _saver = saver;

            _gameModel.StateUpdated += OnGameStateUpdated;
            _gameModel.ObstacleSpawned += OnObstacleSpawned;
            _gameModel.ObstacleRemoved += OnObstacleRemoved;

            _gameView.ScreenTapped += OnScreenTapped;
            _gameView.PlayButtonClicked += OnPlayButtonClicked;
            
            _saver.HighScoreChanged += OnHighScoreChanged;
        }

        private void OnGameStateUpdated(object sender, EventArgs e)
        {
            _gameView.WhiteCharacterPosition = ModelVectorToViewVector(_gameModel.WhiteCharacterPosition);
            _gameView.BlackCharacterPosition = ModelVectorToViewVector(_gameModel.BlackCharacterPosition);
            _gameView.Score = _gameModel.Score;
            _gameView.IsGameOver = _gameModel.IsGameOver;
            _gameView.HasRelevantGameState = true;
        }

        private void OnPlayButtonClicked(object sender, EventArgs e)
        {
            _gameModel.Restart();
        }

        private void OnScreenTapped(object sender, EventArgs e)
        {
            _gameModel.Swap();
        }

        private void OnObstacleSpawned(object sender, ObstacleSpawnedEventArgs e)
        {
            _gameView.CreateObstacleView(e.Obstacle.Id, ModelVectorToViewVector(e.Obstacle.Position), 
                ModelVectorToViewVector(e.Obstacle.Size, 1), GameColorToUnityColor(e.Obstacle.Color));
        }

        private void OnObstacleRemoved(object sender, ObstacleRemovedEventArgs e)
        {
            _gameView.RemoveObstacleView(e.Id);
        }

        private void OnHighScoreChanged(object sender, HighScoreChangedEventArgs e)
        {
            _gameView.HighScore = e.HighScore;
        }

        [Pure]
        private UVector3 ModelVectorToViewVector(SVector2 modelPosition, float x = 0) =>
            new(x, modelPosition.Y, modelPosition.X);

        private Color GameColorToUnityColor(GameColor gameColor) => gameColor switch
        {
            GameColor.White => Color.white,
            GameColor.Black => Color.black,
            GameColor.Red => Color.red,
            _ => throw new ArgumentOutOfRangeException(nameof(gameColor), gameColor, null)
        };
    }
}