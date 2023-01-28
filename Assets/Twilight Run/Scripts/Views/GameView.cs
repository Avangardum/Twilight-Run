using System;
using System.Collections.Generic;
using Avangardum.TwilightRun.Presenters;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Avangardum.TwilightRun.Views
{
    public class GameView : MonoBehaviour, IGameView
    {
        [SerializeField] private GameObject _whiteCharacter;
        [SerializeField] private GameObject _blackCharacter;
        [SerializeField] private GameObject _obstaclePrefab;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private TextMeshProUGUI _gameOverPanelScoreText;
        [SerializeField] private TextMeshProUGUI _gameOverPanelHighScoreText;
        [SerializeField] private Button _gameOverPanelRestartButton;
        [SerializeField] private Button _gameOverPanelMenuButton;

        private Dictionary<int, GameObject> _obstacleViewsById = new();
        private bool _wasGameOverThisFrame;

        public event EventHandler ScreenTapped;
        public event EventHandler PlayButtonClicked;

        public Vector3 WhiteCharacterPosition
        {
            set => _whiteCharacter.transform.position = value;
        }

        public Vector3 BlackCharacterPosition
        {
            set => _blackCharacter.transform.position = value;
        }

        public int Score
        {
            set
            {
                _scoreText.text = value.ToString();
                _gameOverPanelScoreText.text = value.ToString();
            }
        }
        
        public bool IsGameOver
        {
            private get => _gameOverPanel.activeSelf;
            set
            {
                _gameOverPanel.SetActive(value);
                if (value) _wasGameOverThisFrame = true;
            }
        }

        public int HighScore
        {
            set => _gameOverPanelHighScoreText.text = value.ToString();
        }

        public void CreateObstacleView(int id, Vector3 position, Vector3 size, Color color)
        {
            var obstacleView = Instantiate(_obstaclePrefab);
            const string obstacleNameFormat = "Obstacle {0}";
            obstacleView.name = string.Format(obstacleNameFormat, id);
            obstacleView.transform.position = position;
            obstacleView.transform.localScale = size;
            obstacleView.GetComponent<Renderer>().material.color = color;
            _obstacleViewsById.Add(id, obstacleView);
        }

        public void RemoveObstacleView(int id)
        {
            var obstacleView = _obstacleViewsById[id];
            _obstacleViewsById.Remove(id);
            Destroy(obstacleView);
        }

        private void Awake()
        {
            _gameOverPanelRestartButton.onClick.AddListener(OnPlayButtonClicked);
        }

        private void OnPlayButtonClicked()
        {
            PlayButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void Update()
        {
            if (Input.anyKeyDown && !_wasGameOverThisFrame) ScreenTapped?.Invoke(this, EventArgs.Empty);
            if (!IsGameOver) _wasGameOverThisFrame = false;
        }
    }
}