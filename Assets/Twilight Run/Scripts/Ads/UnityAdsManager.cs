using System;
using System.Threading.Tasks;
using Avangardum.TwilightRun.Models;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Avangardum.TwilightRun.Ads
{
    public class UnityAdsManager : IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        private const string AndroidGameId = "5174939";
        private const string IosGameId = null;
        private const string AndroidInterstitialAdId = "Interstitial_Android";
        private const string IosInterstitialAdId = null;
        private const bool TestMode = true;
        private const float MinAdInterval = 120f;
        private static readonly TimeSpan AdShowDelay = TimeSpan.FromSeconds(1);
        
        private IGameModel _gameModel;
        private ISaver _saver;
        private float? _lastAdTime;
        private string _gameId;
        private string _interstitialAdId;

        public UnityAdsManager(IGameModel gameModel, ISaver saver)
        {
            _gameModel = gameModel;
            _saver = saver;
            
            _gameModel.GameOver += OnGameOver;
            
            InitializeAds();
        }

        private bool AreAdsEnabled => !_saver.AreAdsRemoved;

        public void OnInitializationComplete()
        {
            LoadAd();
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.LogError($"Unity ads initialization failed: {error.ToString()} - {message}");
        }
        
        public void OnUnityAdsAdLoaded(string placementId)
        {
            
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Debug.LogError($"Error loading ad: {placementId} - {error.ToString()} - {message}");
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Debug.LogError($"Error showing ad: {placementId} - {error.ToString()} - {message}");
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            _gameModel.IsPaused = true;
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            _gameModel.IsPaused = false;
            _lastAdTime = Time.time;
            LoadAd();
        }
        
        private async void OnGameOver(object sender, EventArgs e)
        {
            if (!AreAdsEnabled) return;
            await Task.Delay(AdShowDelay);
            if (!_lastAdTime.HasValue || Time.time - _lastAdTime.Value >= MinAdInterval)
            {
                ShowAd();
            }
        }

        private void LoadAd()
        {
            Advertisement.Load(_interstitialAdId, this);
        }
        
        private void ShowAd()
        {
            Advertisement.Show(_interstitialAdId, this);
        }

        private void InitializeAds()
        {
            _gameId = Application.platform == RuntimePlatform.IPhonePlayer ? IosGameId : AndroidGameId;
            _interstitialAdId = Application.platform == RuntimePlatform.IPhonePlayer ? IosInterstitialAdId : 
                AndroidInterstitialAdId;
            Advertisement.Initialize(_gameId, TestMode, this);
        }
    }
}