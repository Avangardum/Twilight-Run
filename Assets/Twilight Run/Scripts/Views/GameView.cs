﻿using System;
using Avangardum.TwilightRun.Presenters;
using UnityEngine;

namespace Avangardum.TwilightRun.Views
{
    public class GameView : MonoBehaviour, IGameView
    {
        [SerializeField] private GameObject _whiteCharacter;
        [SerializeField] private GameObject _blackCharacter;
        
        public event EventHandler ScreenTapped;

        public Vector3 WhiteCharacterPosition
        {
            set => _whiteCharacter.transform.position = value;
        }

        public Vector3 BlackCharacterPosition
        {
            set => _blackCharacter.transform.position = value;
        }

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                ScreenTapped?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}