using System;
using Avangardum.TwilightRun.Models;
using UnityEngine;

namespace Avangardum.TwilightRun.Presenters
{
    public interface IGameView
    {
        public event EventHandler ScreenTapped;
        
        public Vector3 WhiteCharacterPosition { set; }
        public Vector3 BlackCharacterPosition { set; }
        public void CreateObstacleView(int id, Vector3 position, Vector3 size, Color color);
        public void RemoveObstacleView(int id);
    }
}