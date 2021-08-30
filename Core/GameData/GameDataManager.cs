using ElementEngine;
using FinalFrontier.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier
{
    public static class GameDataManager
    {
        public static Dictionary<string, StarData> Stars;
        public static Dictionary<string, PlanetData> Planets;
        public static Dictionary<string, MoonData> Moons;
        public static Dictionary<string, AsteroidData> Asteroids;

        public static Dictionary<string, ShipData> Ships;
        public static Dictionary<string, TurretData> Turrets;
        public static Dictionary<string, ProjectileData> Projectiles;

        public static void Load()
        {
            Stars = AssetManager.LoadJSON<Dictionary<string, StarData>>("Data/Stars.json");
            Planets = AssetManager.LoadJSON<Dictionary<string, PlanetData>>("Data/Planets.json");
            Moons = AssetManager.LoadJSON<Dictionary<string, MoonData>>("Data/Moons.json");
            Asteroids = AssetManager.LoadJSON<Dictionary<string, AsteroidData>>("Data/Asteroids.json");

            Ships = AssetManager.LoadJSON<Dictionary<string, ShipData>>("Data/Ships.json");
            Turrets = AssetManager.LoadJSON<Dictionary<string, TurretData>>("Data/Turrets.json");
            Projectiles = AssetManager.LoadJSON<Dictionary<string, ProjectileData>>("Data/Projectiles.json");
        }

    } // GameDataManager
}
