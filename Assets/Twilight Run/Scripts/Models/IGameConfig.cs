namespace Avangardum.TwilightRun.Models
{
    public interface IGameConfig
    {
        float CharacterHorizontalSpeed { get; }
        float CharacterVerticalSpeed { get; }
        float MinCharacterYPosition { get; }
        float MaxCharacterYPosition { get; }
    }
}