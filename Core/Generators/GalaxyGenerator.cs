using ElementEngine;
using ElementEngine.ECS;
using FinalFrontier.GameData;
using SharpNeat.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier
{
    public class GalaxySectorData
    {
        public string ID;
        public Vector2I SectorPosition;
        public Entity Star;
        public List<Entity> Planets = new List<Entity>();
        public List<Entity> Moons = new List<Entity>();
        public List<Entity> Asteroids = new List<Entity>();

        public GalaxySectorData(string id, Vector2I sectorPosition)
        {
            ID = id;
            SectorPosition = sectorPosition;
        }
    }

    public class GalaxyGenerator
    {
        public Dictionary<Vector2I, GalaxySectorData> GalaxyStars = new Dictionary<Vector2I, GalaxySectorData>();

        public Registry Registry;

        public bool ServerMode;
        public string Seed;

        public int GalaxyGridSize = 200;

        private int _minPlanets = 3;
        private int _maxPlanets = 10;
        private int _planetSpacingFromStar = 25000;
        private int _minSpaceBetweenPlanets = 25000;
        private int _maxSpaceBetweenPlanets = 25000;
        private int _planetOrbitRandomSpacing = 5000;

        private int _minMoons = 1;
        private int _maxMoons = 4;
        private int _moonSpacingFromPlanet = 2000;
        private int _spaceBetweenMoons = 1500;
        private int _moonOrbitRandomSpacing = 500;

        private int _asteroidBeltPlanetsSpacing = 2000;
        private int _asteroidBeltThickness = 5000;
        private int _minAsteroids = 250;
        private int _maxAsteroids = 500;

        private int _ringChance = 25;
        private int _minRingSpacingFromPlanet = 1500;
        private int _maxRingSpacingFromPlanet = 2000;
        private int _ringThickness = 250;
        private int _minRingAsteroids = 25;
        private int _maxRingAsteroids = 50;

        private List<StarData> _randomStarList = new List<StarData>();
        private List<PlanetData> _randomPlanetList = new List<PlanetData>();
        private List<MoonData> _randomMoonList = new List<MoonData>();
        private List<AsteroidData> _randomAsteroidList = new List<AsteroidData>();

        public GalaxyGenerator(Registry registry, string seed, bool serverMode)
        {
            Registry = registry;
            Seed = seed;
            ServerMode = serverMode;

            _randomStarList.Clear();

            foreach (var (_, star) in GameDataManager.Stars)
            {
                for (var i = 0; i < star.Weighting; i++)
                    _randomStarList.Add(star);
            }

            _randomPlanetList.Clear();

            foreach (var (_, planet) in GameDataManager.Planets)
            {
                for (var i = 0; i < planet.Weighting; i++)
                    _randomPlanetList.Add(planet);
            }

            _randomMoonList.Clear();

            foreach (var (_, moon) in GameDataManager.Moons)
            {
                for (var i = 0; i < moon.Weighting; i++)
                    _randomMoonList.Add(moon);
            }

            _randomAsteroidList.Clear();

            foreach (var (_, asteroid) in GameDataManager.Asteroids)
            {
                for (var i = 0; i < asteroid.Weighting; i++)
                    _randomAsteroidList.Add(asteroid);
            }

        } // constructor

        public void GenerateGalaxy()
        {
            GalaxyStars.Clear();

            var rng = new FastRandom(MathHelper.GetSeedFromString(Seed));
            var galaxyGrid = new bool[GalaxyGridSize * GalaxyGridSize];
            var stars = 0;

            var center = new Vector2I(GalaxyGridSize / 2);
            var spiralArms = 5;
            var starCount = 100;
            var starsPerArm = starCount / spiralArms;
            var centerRadius = 20;
            var armRadius = 25;
            var spinFactor = 270f;
            var starReserveSize = 4;

            for (var i = 0; i < spiralArms; i++)
            {
                var armStartPos = MathHelper.GetPointOnCircle(center.ToVector2(), centerRadius, i, spiralArms).ToVector2I();

                var armStep = armStartPos - center;
                var armEndPos = armStartPos;

                while (armEndPos.X > 0 && armEndPos.X < GalaxyGridSize
                    && armEndPos.Y > 0 && armEndPos.Y < GalaxyGridSize)
                {
                    armEndPos += armStep;
                }
                armEndPos -= armStep;

                var linePoints = Bresenham.GetLinePoints(armStartPos, armEndPos);

                for (var s = 0; s < starsPerArm; s++)
                {
                    var linePoint = linePoints.GetRandomItem(rng) + new Vector2I(rng.Next(-armRadius, armRadius), rng.Next(-armRadius, armRadius));
                    var distFromCenter = Vector2I.GetDistance(center, linePoint);
                    var percentageFromCenter = distFromCenter / Vector2I.GetDistance(center, armEndPos);
                    var rotationAmount = percentageFromCenter * spinFactor;

                    var rotMatrix = Matrix3x2.CreateTranslation(center.ToVector2() * -1) * Matrix3x2.CreateRotation(rotationAmount.ToRadians()) * Matrix3x2.CreateTranslation(center.ToVector2());
                    linePoint = Vector2.Transform(linePoint.ToVector2(), rotMatrix).ToVector2I();

                    if (linePoint.X < 0 || linePoint.X >= GalaxyGridSize
                        || linePoint.Y < 0 || linePoint.Y >= GalaxyGridSize)
                    {
                        continue;
                    }

                    var index = linePoint.X + GalaxyGridSize * linePoint.Y;
                    galaxyGrid[index] = true;
                }
            }

            // make sure all stars have no other stars within starReserveSize grid cells of them
            for (var y = 0; y < GalaxyGridSize; y++)
            {
                for (var x = 0; x < GalaxyGridSize; x++)
                {
                    var index = x + GalaxyGridSize * y;

                    if (!galaxyGrid[index])
                        continue;

                    // remove stars inside the galaxy center
                    if (Vector2.Distance(new Vector2(x, y), center.ToVector2()) < centerRadius)
                    {
                        galaxyGrid[index] = false;
                        continue;
                    }

                    var nextSquareOffset = 1;
                    var nextSquareSize = 3;

                    while (nextSquareOffset <= starReserveSize)
                    {
                        for (var sqY = 0; sqY < nextSquareSize; sqY++)
                        {
                            for (var sqX = 0; sqX < nextSquareSize; sqX++)
                            {
                                var clearPoint = new Vector2I(x + sqX - nextSquareOffset, y + sqY - nextSquareOffset);

                                if (clearPoint.X < 0 || clearPoint.X >= GalaxyGridSize
                                    || clearPoint.Y < 0 || clearPoint.Y >= GalaxyGridSize)
                                {
                                    continue;
                                }

                                if (clearPoint.X == x && clearPoint.Y == y)
                                    continue;

                                var clearIndex = clearPoint.X + GalaxyGridSize * clearPoint.Y;
                                galaxyGrid[clearIndex] = false;
                            }
                        }

                        nextSquareOffset += 1;
                        nextSquareSize += 2;
                    }
                }
            }

            // generate galaxy star list
            for (var y = 0; y < GalaxyGridSize; y++)
            {
                for (var x = 0; x < GalaxyGridSize; x++)
                {
                    var index = x + GalaxyGridSize * y;

                    if (galaxyGrid[index])
                    {
                        var starPos = new Vector2I(x, y);

                        if (!GalaxyStars.ContainsKey(starPos))
                        {
                            GalaxyStars.Add(starPos, new GalaxySectorData("S" + stars.ToString(), starPos));
                            stars += 1;
                        }
                    }
                }
            }

        } // GenerateGalaxy

    } // GalaxyGenerator
}
