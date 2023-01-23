using System;
using System.Numerics;

namespace Avangardum.TwilightRun.Models
{
    public class GameModel : IGameModel
    {
        private const float CharacterSpeed = 5;
        private const float CharacterVerticalSpeed = 10;
        private const float MaxCharacterYPos = 15;
        private const float MinCharacterYPos = 1;

        private int _whiteCharacterVerticalDirection;
        private Vector2 _whiteCharacterPosition = new(0, MaxCharacterYPos);
        private Vector2 _blackCharacterPosition = new(0, MinCharacterYPos);

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
                var horizontalCharacterMovement = CharacterSpeed * deltaTime;
                var verticalWhiteCharacterMovement = _whiteCharacterVerticalDirection * (CharacterVerticalSpeed * deltaTime);
                var verticalBlackCharacterMovement = -verticalWhiteCharacterMovement;
                var whiteCharacterMovement = new Vector2(horizontalCharacterMovement, verticalWhiteCharacterMovement);
                var blackCharacterMovement = new Vector2(horizontalCharacterMovement, verticalBlackCharacterMovement);
                _whiteCharacterPosition += whiteCharacterMovement;
                _blackCharacterPosition += blackCharacterMovement;
                if (whiteCharacterMovement.Y >= MaxCharacterYPos)
                {
                    _whiteCharacterVerticalDirection = 0;
                    _whiteCharacterPosition.Y = MaxCharacterYPos;
                    _blackCharacterPosition.Y = MinCharacterYPos;
                }
                else if (whiteCharacterMovement.Y <= MinCharacterYPos)
                {
                    _whiteCharacterVerticalDirection = 0;
                    _whiteCharacterPosition.Y = MinCharacterYPos;
                    _blackCharacterPosition.Y = MaxCharacterYPos;
                }
            }
        }

        public void Swap()
        {
            if (_whiteCharacterVerticalDirection != 0) return;
            _whiteCharacterVerticalDirection = _whiteCharacterPosition.Y switch
            {
                MinCharacterYPos => 1,
                MaxCharacterYPos => -1,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}