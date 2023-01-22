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
        }

        private void OnGameStateUpdated(object sender, EventArgs e)
        {
            _gameView.WhiteCharacterPosition = ModelPositionToViewPosition(_gameModel.WhiteCharacterPosition);
            _gameView.BlackCharacterPosition = ModelPositionToViewPosition(_gameModel.BlackCharacterPosition);
        }

        [Pure]
        private UVector3 ModelPositionToViewPosition(SVector2 modelPosition) =>
            new(0, modelPosition.Y, modelPosition.X);
    }
}