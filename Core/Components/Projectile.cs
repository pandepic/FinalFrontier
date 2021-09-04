using ElementEngine.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Components
{
    public struct Projectile
    {
        public Entity Parent;
        public DamageType DamageType;
        public float Damage;
        public float Lifetime;
    }

    public struct Missile
    {
        public float TurnSpeed;
        public float MoveSpeed;
        public Entity Target;
        public float TargetRotation;
    }
}
