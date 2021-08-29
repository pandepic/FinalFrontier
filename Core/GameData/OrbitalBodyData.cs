using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.GameData
{
    public class OrbitalBodyData
    {
        public string Name;
        public string Atlas;
        public string Sprite;
        public int Weighting;
        public float Scale = 1f;
    }

    public class StarData : OrbitalBodyData
    {
    }

    public class PlanetData : OrbitalBodyData
    {
        public bool CanHaveRings = true;
    }

    public class MoonData : OrbitalBodyData
    {
    }

    public class AsteroidData : OrbitalBodyData
    {
    }
}
