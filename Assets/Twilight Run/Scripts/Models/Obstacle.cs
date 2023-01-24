using System.Numerics;

namespace Avangardum.TwilightRun.Models
{
    public class Obstacle
    {
        public Vector2 Position { get; }
        public Vector2 Size { get; }
        public GameColor Color { get; }

        public Obstacle(Vector2 position, Vector2 size, GameColor color)
        {
            Position = position;
            Size = size;
            Color = color;
        }
    }
}