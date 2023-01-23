using System;
using System.Numerics;

namespace Avangardum.TwilightRun.Models
{
    public class GameModel : IGameModel
    {
        private const float CharacterSpeed = 1;
        private const float MaxCharacterYPos = 15;
        private const float MinCharacterYPos = 1;

        public event EventHandler StateUpdated;
        public Vector2 WhiteCharacterPosition { get; private set; } = new(0, MaxCharacterYPos);
        public Vector2 BlackCharacterPosition { get; private set; } = new(0, MinCharacterYPos);

        public void Update(float deltaTime)
        {
            if (deltaTime == 0) return;
            if (deltaTime is < 0 or float.PositiveInfinity or float.NaN)
                throw new ArgumentOutOfRangeException(nameof(deltaTime), deltaTime, $"Invalid {nameof(deltaTime)} value.");

            var characterMovement = Vector2.UnitX * (CharacterSpeed * deltaTime);
            WhiteCharacterPosition += characterMovement;
            BlackCharacterPosition += characterMovement;
            
            StateUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}