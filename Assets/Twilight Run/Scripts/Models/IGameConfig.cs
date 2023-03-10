using System.Collections.Generic;
using System.Numerics;

namespace Avangardum.TwilightRun.Models
{
    public interface IGameConfig
    {
        float CharacterBaseHorizontalSpeed { get; }
        float CharacterHorizontalAcceleration { get; }
        float CharacterMaxHorizontalSpeed { get; }
        float CharacterBaseVerticalSpeed { get; }
        float MinCharacterYPosition { get; }
        float MaxCharacterYPosition { get; }
        float SwapCoyoteTime { get; }
        float StartSafeZoneSize { get; }
        
        // The world generation zone is the area where the world must be always generated
        // ________☺______________
        //  |<---->|<----------->|
        //     |            |
        //   back         forward
        float WorldGenerationZoneForwardSize { get; }
        float WorldGenerationZoneBackSize { get; }
        
        IReadOnlyList<ObstacleGroup> ObstacleGroups { get; }
        Vector2 CharacterSize { get; }
        float ScorePerMeter { get; }
    }
}