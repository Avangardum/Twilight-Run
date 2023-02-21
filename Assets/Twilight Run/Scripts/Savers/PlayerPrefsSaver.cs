using System;
using Avangardum.TwilightRun.Presenters;
using UnityEngine;

namespace Avangardum.TwilightRun.Savers
{
    // ReSharper disable once RedundantNameQualifier
    public class PlayerPrefsSaver : Models.ISaver, Presenters.ISaver, InAppPurchases.ISaver, Ads.ISaver
    {
        private const string HighScoreKey = "High Score";
        private const string AreAdsRemovedKey = "Are Ads Removed";

        public event EventHandler<HighScoreChangedEventArgs> HighScoreChanged;

        public int HighScore
        {
            get => PlayerPrefs.GetInt(HighScoreKey, 0);
            set
            {
                if (HighScore == value) return;
                PlayerPrefs.SetInt(HighScoreKey, value);
                HighScoreChanged?.Invoke(this, new HighScoreChangedEventArgs(value));
            }
        }

        public bool AreAdsRemoved
        {
            get => PlayerPrefs.GetInt(AreAdsRemovedKey) == 1;
            set => PlayerPrefs.SetInt(AreAdsRemovedKey, value ? 1 : 0);
        }

        public void Clear()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}