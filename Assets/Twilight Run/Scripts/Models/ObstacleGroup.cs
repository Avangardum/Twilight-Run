using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Avangardum.TwilightRun.Models
{
    public class ObstacleGroup
    {
        public ObstacleGroup(IEnumerable<Obstacle> obstacles, float size)
        {
            Debug.Assert(obstacles != null);
            var obstaclesList = obstacles.ToList();
            Debug.Assert(size >= obstaclesList.Sum(o => o.Size.X));
            Obstacles = obstaclesList;
            Size = size;
        }

        public List<Obstacle> Obstacles { get; }
        public float Size { get; }
    }
}