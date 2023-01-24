namespace Avangardum.TwilightRun.Models
{
    public interface IGameConfig
    {
        float CharacterHorizontalSpeed { get; }
        float CharacterVerticalSpeed { get; }
        float MinCharacterYPosition { get; }
        float MaxCharacterYPosition { get; }
        float StartSafeZoneSize { get; }
        
        // The world generation zone is the area where the world must be always generated
        // ________☺______________
        //  |<---->|<----------->|
        //     |            |
        //   back         forward
        float WorldGenerationZoneForwardSize { get; }
        float WorldGenerationZoneBackSize { get; }
    }
}