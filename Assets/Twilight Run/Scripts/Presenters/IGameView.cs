using System;
using UnityEngine;

namespace Avangardum.TwilightRun.Presenters
{
    public interface IGameView
    {
        public event EventHandler ScreenTapped;
        
        public Vector3 WhiteCharacterPosition { set; }
        public Vector3 BlackCharacterPosition { set; }
    }
}