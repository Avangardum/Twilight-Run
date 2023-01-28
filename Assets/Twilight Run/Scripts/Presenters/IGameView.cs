using System;
using UnityEngine;

namespace Avangardum.TwilightRun.Presenters
{
    public interface IGameView
    {
        event EventHandler ScreenTapped;
        event EventHandler PlayButtonClicked;
        
        Vector3 WhiteCharacterPosition { set; }
        Vector3 BlackCharacterPosition { set; }
        int Score { set; }
        int HighScore { set; }
        bool IsGameOver { set; }

        void CreateObstacleView(int id, Vector3 position, Vector3 size, Color color);
        void RemoveObstacleView(int id);
    }
}