using System.Numerics;

namespace Avangardum.TwilightRun.Models
{
    public class GameModel : IGameModel
    {
        public const float CharacterSpeed = 1;
        
        public Vector2 WhiteCharacterPosition { get; private set; }
        public Vector2 BlackCharacterPosition { get; private set; }

        public void Update(float deltaTime)
        {
            var characterMovement = Vector2.UnitX * (CharacterSpeed * deltaTime);
            WhiteCharacterPosition += characterMovement;
            BlackCharacterPosition += characterMovement;
        }
    }
}