using System;
using System.Collections.Generic;
using Avangardum.TwilightRun.Presenters;
using UnityEngine;

namespace Avangardum.TwilightRun.Views
{
    public class GameView : MonoBehaviour, IGameView
    {
        [SerializeField] private GameObject _whiteCharacter;
        [SerializeField] private GameObject _blackCharacter;
        [SerializeField] private GameObject _obstaclePrefab;

        private Dictionary<int, GameObject> _obstacleViewsById = new();

        public event EventHandler ScreenTapped;

        public Vector3 WhiteCharacterPosition
        {
            set => _whiteCharacter.transform.position = value;
        }

        public Vector3 BlackCharacterPosition
        {
            set => _blackCharacter.transform.position = value;
        }

        public void CreateObstacleView(int id, Vector3 position, Vector3 size, Color color)
        {
            
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