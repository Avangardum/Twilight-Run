using System.Numerics;

namespace Avangardum.TwilightRun.Models
{
    public static class AabbUtil
    {
        public static bool DoAabbsIntersect(Vector2 aabb1Position, Vector2 aabb1Size,
            Vector2 aabb2Position, Vector2 aabb2Size)
        {
            Vector2 aabb1Min = aabb1Position - aabb1Size / 2;
            Vector2 aabb1Max = aabb1Position + aabb1Size / 2;
            Vector2 aabb2Min = aabb2Position - aabb2Size / 2;
            Vector2 aabb2Max = aabb2Position + aabb2Size / 2;
            return aabb1Min.X <= aabb2Max.X && aabb1Max.X >= aabb2Min.X &&
                   aabb1Min.Y <= aabb2Max.Y && aabb1Max.Y >= aabb2Min.Y;
        }
    }
}