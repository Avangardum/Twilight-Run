using System;
using System.Numerics;
using System.Diagnostics;
using System.Linq;

namespace Avangardum.TwilightRun.Models
{
    public class GameModel : IGameModel
    {
        private int _whiteCharacterVerticalDirection;
        private Vector2 _whiteCharacterPosition;
        private Vector2 _blackCharacterPosition;
        private IGameConfig _gameConfig;

        public GameModel(IGameConfig gameConfig)
        {
            _gameConfig = gameConfig;
            _whiteCharacterPosition = new(0, _gameConfig.MaxCharacterYPosition);
            _blackCharacterPosition = new(0, _gameConfig.MinCharacterYPosition);
        }

        public event EventHandler StateUpdated;

        public Vector2 WhiteCharacterPosition => _whiteCharacterPosition;

        public Vector2 BlackCharacterPosition => _blackCharacterPosition;

        public void Update(float deltaTime)
        {
            if (deltaTime == 0) return;
            if (deltaTime is < 0 or float.PositiveInfinity or float.NaN)
                throw new ArgumentOutOfRangeException(nameof(deltaTime), deltaTime, $"Invalid {nameof(deltaTime)} value.");

            ProcessMovement();

            StateUpdated?.Invoke(this, EventArgs.Empty);

            void ProcessMovement()
            {
                var horizontalCharacterMovement = _gameConfig.CharacterHorizontalSpeed * deltaTime;
                var verticalWhiteCharacterMovement = _whiteCharacterVerticalDirection * (_gameConfig.CharacterVerticalSpeed * deltaTime);
                var verticalBlackCharacterMovement = -verticalWhiteCharacterMovement;
                var whiteCharacterMovement = new Vector2(horizontalCharacterMovement, verticalWhiteCharacterMovement);
                var blackCharacterMovement = new Vector2(horizontalCharacterMovement, verticalBlackCharacterMovement);
                _whiteCharacterPosition += whiteCharacterMovement;
                _blackCharacterPosition += blackCharacterMovement;
                if (_whiteCharacterPosition.Y >= _gameConfig.MaxCharacterYPosition)
                {
                    _whiteCharacterVerticalDirection = 0;
                    _whiteCharacterPosition.Y = _gameConfig.MaxCharacterYPosition;
                    _blackCharacterPosition.Y = _gameConfig.MinCharacterYPosition;
                }
                else if (_whiteCharacterPosition.Y <= _gameConfig.MinCharacterYPosition)
                {
                    _whiteCharacterVerticalDirection = 0;
                    _whiteCharacterPosition.Y = _gameConfig.MinCharacterYPosition;
                    _blackCharacterPosition.Y = _gameConfig.MaxCharacterYPosition;
                }
            }
        }

        public void Swap()
        {
            if (_whiteCharacterVerticalDirection != 0) return;
            Debug.Assert(new[] { _gameConfig.MinCharacterYPosition, _gameConfig.MaxCharacterYPosition }.
                Contains(_whiteCharacterPosition.Y));
            _whiteCharacterVerticalDirection = _whiteCharacterPosition.Y == _gameConfig.MinCharacterYPosition ? 1 : -1;
        }
    }
}