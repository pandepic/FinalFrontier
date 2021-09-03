using ElementEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Components
{
    public struct AISentry
    {
        public Vector2I Sector;
        public Vector2 SentryPosition;
        public float MaxEnemySearchRange;
        public float MaxEnemyChaseRange;
    }
}
