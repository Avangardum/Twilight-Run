using System;
using System.Collections.Generic;
using System.Linq;
using Avangardum.TwilightRun.Presenters;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Avangardum.TwilightRun.Views
{
    public class GameView : MonoBehaviour, IGameView
    {
        private static readonly int HorizontalSpeedHash = Animator.StringToHash("Horizontal Speed");
        private static readonly int IsFallingHash = Animator.StringToHash("Is Falling");
        
        [SerializeField] private GameObject _whiteCharacter;
        [SerializeField] private GameObject _blackCharacter;
        [SerializeField] private GameObject _obstaclePrefab;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private TextMeshProUGUI _gameOverPanelScoreText;
        [SerializeField] private TextMeshProUGUI _gameOverPanelHighScoreText;
        [SerializeField] private Button _gameOverPanelRestartButton;
        [SerializeField] private Button _gameOverPanelMenuButton;

        private bool _wasGameOverThisFrame;
        private readonly Dictionary<int, GameObject> _obstacleViewsById = new();
        private Dictionary<GameObject, Animator> _characterAnimators;
        private bool _wasInitializedWithFirstRelevantGameState;
        private Vector3? _whiteCharacterPreviousPosition;
        private float _minCharacterYPosition;
        private float _maxCharacterYPosition;

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

        public bool HasRelevantGameState
        {
            set
            {
                if (value && !_wasInitializedWithFirstRelevantGameState)
                    InitializeWithFirstRelevantGameState();
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

        private void InitializeWithFirstRelevantGameState()
        {
            var characterYPositions = new List<float> { _whiteCharacter.transform.position.y, 
                _blackCharacter.transform.position.y };
            _minCharacterYPosition = characterYPositions.Min();
            _maxCharacterYPosition = characterYPositions.Max();
            _wasInitializedWithFirstRelevantGameState = true;
        }

        private void OnPlayButtonClicked()
        {
            PlayButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void Awake()
        {
            _gameOverPanelRestartButton.onClick.AddListener(OnPlayButtonClicked);
            _characterAnimators = new Dictionary<GameObject, Animator>
            {
                [_whiteCharacter] = _whiteCharacter.GetComponentInChildren<Animator>(),
                [_blackCharacter] = _blackCharacter.GetComponentInChildren<Animator>()
            };
        }

        private void Update()
        {
            if (Input.anyKeyDown && !_wasGameOverThisFrame) ScreenTapped?.Invoke(this, EventArgs.Empty);
            if (!IsGameOver) _wasGameOverThisFrame = false;
            SetCharacterAnimatorParameters();
            SetCharacterRotation();

            _whiteCharacterPreviousPosition = _whiteCharacter.transform.position;

            void SetCharacterAnimatorParameters()
            {
                if (_whiteCharacterPreviousPosition == null) return;
                var whiteCharacterMovement = _whiteCharacter.transform.position - _whiteCharacterPreviousPosition;
                var horizontalMovement = whiteCharacterMovement.Value.z;
                var horizontalSpeed = horizontalMovement / Time.deltaTime;
                var characterAnimators = _characterAnimators.Values.ToList();
                characterAnimators.ForEach(a => a.SetFloat(HorizontalSpeedHash, horizontalSpeed));
                var whiteCharacterVerticalMovement = whiteCharacterMovement.Value.y;
                var isFalling = whiteCharacterVerticalMovement != 0;
                characterAnimators.ForEach(a => a.SetBool(IsFallingHash, isFalling));
            }

            void SetCharacterRotation()
            {
                if (_whiteCharacterPreviousPosition == null) return;
                var whiteCharacterYPosition = _whiteCharacter.transform.position.y;
                var whiteCharacterVerticalMovement = whiteCharacterYPosition - _whiteCharacterPreviousPosition.Value.y;
                var whiteCharacterVerticalDirection = Mathf.Sign(whiteCharacterVerticalMovement);
                if (whiteCharacterVerticalDirection == 0) return;
                var whiteCharacterSwapStartYPosition = whiteCharacterVerticalDirection switch
                {
                    1 => _minCharacterYPosition,
                    -1 => _maxCharacterYPosition,
                    _ => throw new ArgumentOutOfRangeException()
                };
                var whiteCharacterSwapEndYPosition = whiteCharacterVerticalDirection switch
                {
                    1 => _maxCharacterYPosition,
                    -1 => _minCharacterYPosition,
                    _ => throw new ArgumentOutOfRangeException()
                };
                var swapProgress = Mathf.InverseLerp(whiteCharacterSwapStartYPosition, whiteCharacterSwapEndYPosition,
                    whiteCharacterYPosition);
                const float swapProgressToStartZRotation = 0.1f;
                const float swapProgressToEndZRotation = 0.5f;
                var zRotationProgress = Mathf.InverseLerp(swapProgressToStartZRotation, swapProgressToEndZRotation,
                    swapProgress);
                var whiteCharacterSwapStartZRotation = whiteCharacterVerticalDirection switch
                {
                    1 => 0,
                    -1 => 180,
                    _ => throw new ArgumentOutOfRangeException()
                };
                var whiteCharacterSwapEndZRotation = whiteCharacterVerticalDirection switch
                {
                    1 => 180,
                    -1 => 0,
                    _ => throw new ArgumentOutOfRangeException()
                };
                var whiteCharacterZRotation = Mathf.Lerp(whiteCharacterSwapStartZRotation, whiteCharacterSwapEndZRotation, zRotationProgress);
                var blackCharacterZRotation = 180 - whiteCharacterZRotation;
                _whiteCharacter.transform.eulerAngles = new Vector3(0, 0, whiteCharacterZRotation);
                _blackCharacter.transform.eulerAngles = new Vector3(0, 0, blackCharacterZRotation);
            }
        }
    }
}