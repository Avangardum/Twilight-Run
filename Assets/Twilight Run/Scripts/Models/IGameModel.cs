using System.Numerics;

namespace Avangardum.TwilightRun.Models
{
    public interface IGameModel
    {
        Vector2 WhiteCharacterPosition { get; }
        Vector2 BlackCharacterPosition { get; }

        void Update(float deltaTime);
    }
}