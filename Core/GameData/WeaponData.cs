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
        public ProjectileDamageType DamageType;
        public string ResourceAmmo;
        public bool UseAmmo;
        public string Atlas;
        public string Sprite;
        public float Speed;
        public float TurnSpeed;
        public Vector4 Colour;
        public float Scale;
    }

    public class WeaponData
    {
        public string Name;
        public ClassType Class;
        public float Damage;
        public float TurnSpeed;
        public float MaxFiringAngle;
        public float MinRange;
        public float MaxRange;
        public float Cooldown; // in seconds

        public int BurstCount;
        public float BurstSpacing; // in seconds

        public string Atlas;
        public string Sprite;
        public float Scale;

        public string Projectile;
        public Vector2 ProjectileSpawnPosition;
        public float AmmoPerShot;

        public List<ClassType> TargetPriorities = new List<ClassType>();
    }
}
