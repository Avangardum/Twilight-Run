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

        public GamePresenter(IGameModel gameModel, IGameView gameView)
        {
            _gameModel = gameModel;
            _gameView = gameView;

            gameModel.StateUpdated += OnGameStateUpdated;
            gameModel.ObstacleSpawned += OnObstacleSpawned;
            gameModel.ObstacleRemoved += OnObstacleRemoved;

            gameView.ScreenTapped += OnScreenTapped;
        }

        private void OnGameStateUpdated(object sender, EventArgs e)
        {
            _gameView.WhiteCharacterPosition = ModelVectorToViewVector(_gameModel.WhiteCharacterPosition);
            _gameView.BlackCharacterPosition = ModelVectorToViewVector(_gameModel.BlackCharacterPosition);
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

        [Pure]
        private UVector3 ModelVectorToViewVector(SVector2 modelPosition, float x = 0) =>
            new(x, modelPosition.Y, modelPosition.X);

        [Pure]
        private Color GameColorToUnityColor(GameColor gameColor) => gameColor switch
        {
            GameColor.White => Color.white,
            GameColor.Black => Color.black,
            GameColor.Red => Color.red,
            _ => throw new ArgumentOutOfRangeException(nameof(gameColor), gameColor, null)
        };
    }
}