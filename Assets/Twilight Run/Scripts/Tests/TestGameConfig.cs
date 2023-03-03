using System.Collections.Generic;
using System.Numerics;
using Avangardum.TwilightRun.Models;

namespace Avangardum.TwilightRun.Tests
{
    public class TestGameConfig : IGameConfig
    {
        public TestGameConfig()
        {
            
        }
        
        public TestGameConfig(Models.IGameConfig sourceConfig)
        {
            CharacterSize = sourceConfig.CharacterSize;
            ScorePerMeter = sourceConfig.ScorePerMeter;
            CharacterBaseHorizontalSpeed = sourceConfig.CharacterBaseHorizontalSpeed;
            CharacterMaxHorizontalSpeed = sourceConfig.CharacterMaxHorizontalSpeed;
            CharacterHorizontalAcceleration = sourceConfig.CharacterHorizontalAcceleration;
            CharacterBaseVerticalSpeed = sourceConfig.CharacterBaseVerticalSpeed;
            MinCharacterYPosition = sourceConfig.MinCharacterYPosition;
            MaxCharacterYPosition = sourceConfig.MaxCharacterYPosition;
            SwapCoyoteTime = sourceConfig.SwapCoyoteTime;
            StartSafeZoneSize = sourceConfig.StartSafeZoneSize;
            WorldGenerationZoneForwardSize = sourceConfig.WorldGenerationZoneForwardSize;
            WorldGenerationZoneBackSize = sourceConfig.WorldGenerationZoneBackSize;
            ObstacleGroups.AddRange(sourceConfig.ObstacleGroups);
        }

        public Vector2 CharacterSize { get; }
        public float ScorePerMeter { get; }
        public float CharacterBaseHorizontalSpeed { get; set; }
        public float CharacterHorizontalAcceleration { get; set; }
        public float CharacterMaxHorizontalSpeed { get; set; }
        public float CharacterBaseVerticalSpeed { get; set; }
        public float MinCharacterYPosition { get; set; }
        public float MaxCharacterYPosition { get; set; }
        public float SwapCoyoteTime { get; set; }
        public float StartSafeZoneSize { get; set; }
        public float WorldGenerationZoneForwardSize { get; set; }
        public float WorldGenerationZoneBackSize { get; set; }
        public List<ObstacleGroup> ObstacleGroups { get; } = new();
        IReadOnlyList<ObstacleGroup> IGameConfig.ObstacleGroups => ObstacleGroups;
    }
}