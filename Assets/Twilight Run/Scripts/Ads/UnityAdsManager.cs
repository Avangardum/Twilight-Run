using System;
using Avangardum.TwilightRun.Models;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Avangardum.TwilightRun.Ads
{
    public class UnityAdsManager : IUnityAdsInitializationListener
    {
        private const string AndroidGameId = "5174939";
        private const string IosGameId = null;
        private const bool TestMode = true;
        private const float MinAdInterval = 120f;
        
        private IGameModel _gameModel;
        private float? _lastAdTime;
        private string _gameId;
        
        public UnityAdsManager(IGameModel gameModel)
        {
            _gameModel = gameModel;
            
            _gameModel.GameOver += OnGameOver;
            
            InitializeAds();
        }

        public void OnInitializationComplete()
        {
            Debug.Log("Unity Ads initialization complete.");
        }
 
        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
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

        private void InitializeAds()
        {
            _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
                ? IosGameId
                : AndroidGameId;
            Advertisement.Initialize(_gameId, TestMode, this);
        }
    }
}