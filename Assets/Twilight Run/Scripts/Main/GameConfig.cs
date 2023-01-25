using System.Collections.Generic;
using Avangardum.TwilightRun.Models;
using UnityEngine;

namespace Avangardum.TwilightRun.Main
{
    public class GameConfig : ScriptableObject, IGameConfig
    {
        [field: SerializeField] public float CharacterHorizontalSpeed { get; private set; }
        [field: SerializeField] public float CharacterVerticalSpeed { get; private set; }
        [field: SerializeField] public float MinCharacterYPosition { get; private set; }
        [field: SerializeField] public float MaxCharacterYPosition { get; private set; }
        [field: SerializeField] public float StartSafeZoneSize { get; private set; }
        [field: SerializeField] public float WorldGenerationZoneForwardSize { get; private set; }
        [field: SerializeField] public float WorldGenerationZoneBackSize { get; private set; }
        [SerializeField] private List<SerializableObstacleGroup> _obstacleGroups = new();
        public IReadOnlyList<ObstacleGroup> ObstacleGroups => _obstacleGroups.ConvertAll(g => g.ToObstacleGroup());
    }
}