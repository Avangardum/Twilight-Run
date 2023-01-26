﻿using System.Collections.Generic;
using Avangardum.TwilightRun.Models;
using UnityEngine;
using SVector2 = System.Numerics.Vector2;
using UVector2 = UnityEngine.Vector2;

namespace Avangardum.TwilightRun.Main
{
    public class GameConfig : ScriptableObject, IGameConfig
    {
        [SerializeField] private UVector2 _characterSize;
        [field: SerializeField] public float CharacterBaseHorizontalSpeed { get; private set; }
        [field: SerializeField] public float CharacterAcceleration { get; private set; }
        [field: SerializeField] public float CharacterMaxHorizontalSpeed { get; private set; }
        [field: SerializeField] public float CharacterVerticalSpeed { get; private set; }
        [field: SerializeField] public float MinCharacterYPosition { get; private set; }
        [field: SerializeField] public float MaxCharacterYPosition { get; private set; }
        [field: SerializeField] public float StartSafeZoneSize { get; private set; }
        [field: SerializeField] public float WorldGenerationZoneForwardSize { get; private set; }
        [field: SerializeField] public float WorldGenerationZoneBackSize { get; private set; }
        [field: SerializeField] public float ScorePerMeter { get; private set; }
        [SerializeField] private List<SerializableObstacleGroup> _obstacleGroups = new();
        public IReadOnlyList<ObstacleGroup> ObstacleGroups => _obstacleGroups.ConvertAll(g => g.ToObstacleGroup());
        public SVector2 CharacterSize => new(_characterSize.x, _characterSize.y);
    }
}