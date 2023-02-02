using System;
using System.Collections.Generic;
using System.Linq;
using Avangardum.TwilightRun.Presenters;
using UnityEngine;
using TMPro;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Avangardum.TwilightRun.Views
{
    public class GameView : MonoBehaviour, IGameView
    {
        private static readonly int HorizontalSpeedHash = Animator.StringToHash("Horizontal Speed");
        private static readonly int IsFallingHash = Animator.StringToHash("Is Falling");
        private static readonly int RunningHash = Animator.StringToHash("Running");
        private const float CharacterHorizontalDistanceFromEnvironmentSectionToMoveIt = 210f;
        private const float EnvironmentSectionShiftPerMove = 400f;
        
        [SerializeField] private GameObject _whiteCharacter;
        [SerializeField] private GameObject _blackCharacter;
        [SerializeField] private GameObject _whiteObstaclePrefab;
        [SerializeField] private GameObject _blackObstaclePrefab;
        [SerializeField] private GameObject _redObstaclePrefab;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private TextMeshProUGUI _gameOverPanelScoreText;
        [SerializeField] private TextMeshProUGUI _gameOverPanelHighScoreText;
        [SerializeField] private Button _gameOverPanelRestartButton;
        [SerializeField] private Button _gameOverPanelMenuButton;
        [SerializeField] private GameObject _mainMenuPanel;
        [SerializeField] private Button _mainMenuPanelPlayButton;
        [SerializeField] private List<GameObject> _environmentSections;

        private bool _isGameOver = true;
        private bool _wasGameOverThisFrame = true;
        private readonly Dictionary<int, GameObject> _obstacleViewsById = new();
        private Dictionary<GameObject, Animator> _characterAnimators;
        private bool _wasInitializedWithFirstRelevantGameState;
        private Vector3? _whiteCharacterPreviousPosition;
        private float _minCharacterYPosition;
        private float _maxCharacterYPosition;
        private RagdollControl _whiteCharacterRagdollControl;
        private RagdollControl _blackCharacterRagdollControl;
        private List<Vector3> _environmentSectionDefaultPositions;

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
            private get => _isGameOver;
            set
            {
                _isGameOver = value;
                _gameOverPanel.SetActive(value);
                _whiteCharacterRagdollControl.IsRagdoll = value;
                _blackCharacterRagdollControl.IsRagdoll = value;
                if (value)
                {
                    SetRagdollGravity();
                    SetRagdollSpeed();
                }
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
        
        private Vector3 WhiteCharacterMovementThisFrame => 
            _whiteCharacter.transform.position - _whiteCharacterPreviousPosition ?? Vector3.zero;
        
        private Vector3 WhiteCharacterSpeed => WhiteCharacterMovementThisFrame / Time.deltaTime;

        public void CreateObstacleView(int id, Vector3 position, Vector3 size, Color color)
        {
            var obstacleView = Instantiate(GetObstaclePrefabForColor(color));
            const string obstacleNameFormat = "Obstacle {0}";
            obstacleView.name = string.Format(obstacleNameFormat, id);
            obstacleView.transform.position = position;
            obstacleView.transform.localScale = size;
            var obstacleViewQuadBox = obstacleView.GetComponent<QuadBox>();
            Assert.IsNotNull(obstacleViewQuadBox);
            _obstacleViewsById.Add(id, obstacleView);
        }

        private GameObject GetObstaclePrefabForColor(Color color)
        {
            if (color == Color.white) return _whiteObstaclePrefab;
            if (color == Color.black) return _blackObstaclePrefab;
            if (color == Color.red) return _redObstaclePrefab;
            throw new ArgumentException($"Unknown color {color}");
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
        
        private void SetRagdollGravity()
        {
            var isWhiteCharacterGravityInversed = Math.Sign(WhiteCharacterMovementThisFrame.y) switch
            {
                1 => true,
                -1 => false,
                0 => _whiteCharacter.transform.position.y == _maxCharacterYPosition,
                _ => throw new ArgumentOutOfRangeException()
            };
            var isBlackCharacterGravityInversed = !isWhiteCharacterGravityInversed;
            _whiteCharacterRagdollControl.IsGravityInversed = isWhiteCharacterGravityInversed;
            _blackCharacterRagdollControl.IsGravityInversed = isBlackCharacterGravityInversed;
        }

        private void SetRagdollSpeed()
        {
            _whiteCharacterRagdollControl.Velocity = WhiteCharacterSpeed;
            var blackCharacterVelocity = WhiteCharacterSpeed;
            blackCharacterVelocity.y *= -1;
            _blackCharacterRagdollControl.Velocity = blackCharacterVelocity;
        }

        private void OnGameRestarted()
        {
            _whiteCharacterPreviousPosition = null;
            _characterAnimators.Values.ToList().ForEach(a => a.Play(RunningHash));
            foreach (var (environmentSection, defaultPos) in 
                _environmentSections.Zip(_environmentSectionDefaultPositions, (a, b) => (a, b)))
            {
                environmentSection.transform.position = defaultPos;
            }
        }

        private void OnPlayButtonClicked()
        {
            PlayButtonClicked?.Invoke(this, EventArgs.Empty);
            _mainMenuPanel.SetActive(false);
            OnGameRestarted();
        }

        private void OnMainMenuButtonClicked()
        {
            _mainMenuPanel.SetActive(true);
            _gameOverPanel.SetActive(false);
        }

        private void Awake()
        {
            _gameOverPanelRestartButton.onClick.AddListener(OnPlayButtonClicked);
            _mainMenuPanelPlayButton.onClick.AddListener(OnPlayButtonClicked);
            _gameOverPanelMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
            _characterAnimators = new Dictionary<GameObject, Animator>
            {
                [_whiteCharacter] = _whiteCharacter.GetComponentInChildren<Animator>(),
                [_blackCharacter] = _blackCharacter.GetComponentInChildren<Animator>()
            };
            _whiteCharacterRagdollControl = _whiteCharacter.GetComponentInChildren<RagdollControl>();
            _blackCharacterRagdollControl = _blackCharacter.GetComponentInChildren<RagdollControl>();
            _environmentSectionDefaultPositions = _environmentSections.Select(s => s.transform.position).ToList();
        }
        
        private void Update()
        {
            if (Input.anyKeyDown && !_wasGameOverThisFrame) ScreenTapped?.Invoke(this, EventArgs.Empty);
            if (!IsGameOver) _wasGameOverThisFrame = false;
            SetCharacterAnimatorParameters();
            SetCharacterRotation();
            MoveEnvironmentSectionIfNecessary();
            
            _whiteCharacterPreviousPosition = _whiteCharacter.transform.position;

            void SetCharacterAnimatorParameters()
            {
                if (_whiteCharacterPreviousPosition == null) return;
                var characterAnimators = _characterAnimators.Values.ToList();
                characterAnimators.ForEach(a => a.SetFloat(HorizontalSpeedHash, WhiteCharacterSpeed.z));
                var isFalling = WhiteCharacterMovementThisFrame.y != 0;
                characterAnimators.ForEach(a => a.SetBool(IsFallingHash, isFalling));
            }

            void SetCharacterRotation()
            {
                if (_whiteCharacterPreviousPosition == null) return;
                var whiteCharacterVerticalDirection = Mathf.Sign(WhiteCharacterMovementThisFrame.y);
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
                    _whiteCharacter.transform.position.y);
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
            
            void MoveEnvironmentSectionIfNecessary()
            {
                var characterHorizontalDistanceFromBackSection = _whiteCharacter.transform.position.z -
                    _environmentSections[0].transform.position.z;
                if (characterHorizontalDistanceFromBackSection >= CharacterHorizontalDistanceFromEnvironmentSectionToMoveIt)
                {
                    _environmentSections[0].transform.Translate(Vector3.forward * EnvironmentSectionShiftPerMove, Space.World);
                    (_environmentSections[0], _environmentSections[1]) = (_environmentSections[1], _environmentSections[0]);
                }
            }
        }
    }
}