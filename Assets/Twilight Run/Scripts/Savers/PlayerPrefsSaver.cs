using Avangardum.TwilightRun.Models;
using UnityEngine;

namespace Avangardum.TwilightRun.Savers
{
    public class PlayerPrefsSaver : ISaver
    {
        private const string HighScoreKey = "High Score";
        
        public int HighScore
        {
            get => PlayerPrefs.GetInt(HighScoreKey, 0);
            set => PlayerPrefs.SetInt(HighScoreKey, value);
        }

        public void Clear()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}