using System.Collections.Generic;
using System.Linq;

namespace Avangardum.TwilightRun.Models
{
    public class ObstacleGroup
    {
        public ObstacleGroup(IEnumerable<Obstacle> obstacles, float size, int weight = 1, int difficulty = 0)
        {
            Obstacles = obstacles.ToList();
            Size = size;
            Weight = weight;
            Difficulty = difficulty;
        }

        public List<Obstacle> Obstacles { get; }
        public float Size { get; }
        public int Weight { get; }
        public int Difficulty { get; }
    }
}