using System;
using System.Collections.Generic;
using System.Numerics;

namespace Avangardum.TwilightRun.Models
{
    public interface IGameModel
    {
        event EventHandler StateUpdated;
        
        Vector2 WhiteCharacterPosition { get; }
        Vector2 BlackCharacterPosition { get; }
        IReadOnlyList<Obstacle> Obstacles { get; }

        void Update(float deltaTime);
        void Swap();
    }
}