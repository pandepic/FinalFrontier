using ElementEngine;
using ElementEngine.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Components
{
    public struct Orbit
    {
        public Entity Parent;
        public float Radius;
        public float Speed;
        public int StartIndex;
    }

    public struct OrbitalBody
    {
        public Entity Parent;
        public string ID;
        public Vector2I Sector;
    }

    public struct Star { }
    public struct Planet { }
    public struct Colonisable { }
    public struct Moon { }
    public struct Asteroid { }

    public struct RingAsteroid
    {
        public Entity Parent;

        public RingAsteroid(Entity parent)
        {
            Parent = parent;
        }
    }
}
