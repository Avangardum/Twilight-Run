using System;
using Avangardum.TwilightRun.Models;
using UnityEngine;

namespace Avangardum.TwilightRun.Ads
{
    public class AdManager
    {
        private const float MinAdInterval = 120f;
        
        private IGameModel _gameModel;
        private float? _lastAdTime;
        
        public AdManager(IGameModel gameModel)
        {
            _gameModel = gameModel;
            
            gameModel.GameOver += OnGameOver;
        }

        private void OnGameOver(object sender, EventArgs e)
        {
            if (!_lastAdTime.HasValue || Time.time - _lastAdTime.Value >= MinAdInterval)
            {
                ShowAd();
            }
        }

        private void ShowAd()
        {
            Debug.Log("Showing ad");
            _lastAdTime = Time.time;
        }
    }
}