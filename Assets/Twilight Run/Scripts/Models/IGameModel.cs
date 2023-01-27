using System;
using System.Collections.Generic;
using System.Numerics;

namespace Avangardum.TwilightRun.Models
{
    public interface IGameModel
    {
        event EventHandler StateUpdated;
        event EventHandler<ObstacleSpawnedEventArgs> ObstacleSpawned;

        Vector2 WhiteCharacterPosition { get; }
        Vector2 BlackCharacterPosition { get; }
        IReadOnlyList<Obstacle> Obstacles { get; }
        bool IsGameOver { get; }
        int Score { get; }

        void Update(float deltaTime);
        void Swap();
        event EventHandler<ObstacleRemovedEventArgs> ObstacleRemoved;
        void Restart();
    }
}