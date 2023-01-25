using System.Collections.Generic;
using Avangardum.TwilightRun.Models;

namespace Avangardum.TwilightRun.Tests
{
    public class TestGameConfig : IGameConfig
    {
        public TestGameConfig()
        {
            
        }
        
        public TestGameConfig(IGameConfig sourceConfig)
        {
            CharacterHorizontalSpeed = sourceConfig.CharacterHorizontalSpeed;
            CharacterVerticalSpeed = sourceConfig.CharacterVerticalSpeed;
            MinCharacterYPosition = sourceConfig.MinCharacterYPosition;
            MaxCharacterYPosition = sourceConfig.MaxCharacterYPosition;
            StartSafeZoneSize = sourceConfig.StartSafeZoneSize;
            WorldGenerationZoneForwardSize = sourceConfig.WorldGenerationZoneForwardSize;
            WorldGenerationZoneBackSize = sourceConfig.WorldGenerationZoneBackSize;
            ObstacleGroups.AddRange(sourceConfig.ObstacleGroups);
        }
        
        public float CharacterHorizontalSpeed { get; set; }
        public float CharacterVerticalSpeed { get; set; }
        public float MinCharacterYPosition { get; set; }
        public float MaxCharacterYPosition { get; set; }
        public float StartSafeZoneSize { get; set; }
        public float WorldGenerationZoneForwardSize { get; set; }
        public float WorldGenerationZoneBackSize { get; set; }
        public List<ObstacleGroup> ObstacleGroups { get; } = new();
        IReadOnlyList<ObstacleGroup> IGameConfig.ObstacleGroups => ObstacleGroups;
    }
}