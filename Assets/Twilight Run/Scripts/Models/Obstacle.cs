using System;
using System.Numerics;

namespace Avangardum.TwilightRun.Models
{
    public class Obstacle
    {
        private static int _nextId;
        
        public int Id { get; }
        public Vector2 Position { get; }
        public Vector2 Size { get; }
        public GameColor Color { get; }

        public Obstacle(Vector2 position, Vector2 size, GameColor color)
        {
            Id = _nextId++;
            Position = position;
            Size = size;
            Color = color;
        }
    }
}