using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.GameData
{
    public class ProjectileData
    {
        public string Name;
        public ProjectileType Type;
        public string Atlas;
        public string Sprite;
        public Vector4 Colour;
        public float Scale;
    }

    public class TurretData
    {
        public string Name;
        public ClassType Class;
        public string Atlas;
        public string Sprite;
        public float Scale;
        public Vector2 ProjectileSpawnPosition;
    }
}
