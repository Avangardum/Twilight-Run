using System;
using Avangardum.TwilightRun.Presenters;
using UnityEngine;

namespace Avangardum.TwilightRun.Savers
{
    // ReSharper disable once RedundantNameQualifier
    public class PlayerPrefsSaver : Models.ISaver, Presenters.ISaver
    {
        private const string HighScoreKey = "High Score";

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

        public void Clear()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}