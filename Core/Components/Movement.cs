using ElementEngine;
using ElementEngine.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Components
{
    public struct MoveToPosition
    {
        public Vector2 Position;
        public Vector2I SectorPosition;
        public bool Orbit;
    }

    public struct MoveToEntity
    {
        public Entity Target;
        public bool Orbit;
    }
}
