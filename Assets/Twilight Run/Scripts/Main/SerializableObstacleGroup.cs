using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Avangardum.TwilightRun.Models;
using UnityEngine;
using SVector2 = System.Numerics.Vector2;
using UVector2 = UnityEngine.Vector2;

namespace Avangardum.TwilightRun.Main
{
    [Serializable]
    public class SerializableObstacleGroup
    {
        [Serializable]
        private class SerializableObstacle
        {
            public UVector2 Position;
            public UVector2 Size = Vector2.one;
            public GameColor Color;
        }

        [SerializeField] private string _name;
        [SerializeField] private List<SerializableObstacle> _obstacles;
        [SerializeField] private float _size;
        [SerializeField] private int _weight = 1;
        [SerializeField] private int _difficulty;
        
        public ObstacleGroup ToObstacleGroup()
        {
            var obstacles = _obstacles.ConvertAll(o => 
                new Obstacle(UVector2ToSVector2(o.Position), UVector2ToSVector2(o.Size), o.Color));
            return new ObstacleGroup(obstacles, _size, _weight, _difficulty);
        }
        
        [Pure]
        public SVector2 UVector2ToSVector2(UVector2 uVector2)
        {
            return new SVector2(uVector2.x, uVector2.y);
        }
    }
}