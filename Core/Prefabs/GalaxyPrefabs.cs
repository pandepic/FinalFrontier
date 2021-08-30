using ElementEngine;
using ElementEngine.ECS;
using FinalFrontier.Components;
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
    public static class GalaxyPrefabs
    {
        private static Vector2 GetOrbitPosition(float orbit)
        {
            return MathHelper.GetPointOnCircle(Vector2.Zero, orbit, 0, 200);
        }

        public static Entity BuildStar(Registry registry, string id, Vector2I sectorPosition, Vector2 position, StarData data, int drawLayer)
        {
            var texture = AssetManager.LoadTexture2D(data.Sprite);
            var entity = registry.CreateEntity();

            entity.TryAddComponent(new Star());
            entity.TryAddComponent(new Transform()
            {
                Rotation = 0f,
                SectorPosition = sectorPosition,
                Position = position,
            });
            entity.TryAddComponent(new OrbitalBody()
            {
                ID = id,
                Sector = sectorPosition,
            });
            entity.TryAddComponent(new Drawable()
            {
                AtlasRect = new Rectangle(Vector2I.Zero, texture.Size),
                Origin = texture.SizeF / 2f,
                Scale = new Vector2(data.Scale),
                Texture = data.Sprite,
                TextureReference = texture,
                Layer = drawLayer,
                MinSize = new Vector2(10),
                MaxZoomLevel = 0,
            });

            return entity;

        } // BuildStar

        public static Entity BuildPlanet(Registry registry, string id, Vector2I sectorPosition, Entity parent, float orbit, PlanetData data, FastRandom rng, int drawLayer)
        {
            var texture = AssetManager.LoadTexture2D(data.Sprite);
            var entity = registry.CreateEntity();

            entity.TryAddComponent(new Planet());
            entity.TryAddComponent(new Transform()
            {
                Rotation = 0f,
                Position = GetOrbitPosition(orbit),
                Parent = parent,
            });
            entity.TryAddComponent(new Orbit()
            {
                Parent = parent,
                Speed = rng.Next(20, 50),
                Radius = orbit,
            });
            entity.TryAddComponent(new OrbitalBody()
            {
                ID = id,
                Parent = parent,
                Sector = sectorPosition,
            });
            entity.TryAddComponent(new Drawable()
            {
                AtlasRect = new Rectangle(Vector2I.Zero, texture.Size),
                Origin = texture.SizeF / 2f,
                Scale = new Vector2(data.Scale),
                Texture = data.Sprite,
                TextureReference = AssetManager.LoadTexture2D(data.Sprite),
                Layer = drawLayer,
                MinSize = new Vector2(2),
                MaxZoomLevel = 14,
            });

            return entity;

        } // BuildPlanet

        public static Entity BuildMoon(Registry registry, string id, Vector2I sectorPosition, Entity parent, float orbit, MoonData data, FastRandom rng, int drawLayer)
        {
            var texture = AssetManager.LoadTexture2D(data.Sprite);
            var entity = registry.CreateEntity();

            entity.TryAddComponent(new Moon());
            entity.TryAddComponent(new Transform()
            {
                Rotation = 0f,
                Position = GetOrbitPosition(orbit),
                Parent = parent,
            });
            entity.TryAddComponent(new Orbit()
            {
                Parent = parent,
                Speed = rng.Next(20, 50),
                Radius = orbit,
            });
            entity.TryAddComponent(new OrbitalBody()
            {
                ID = id,
                Parent = parent,
                Sector = sectorPosition,
            });
            entity.TryAddComponent(new Drawable()
            {
                AtlasRect = new Rectangle(Vector2I.Zero, texture.Size),
                Origin = texture.SizeF / 2f,
                Scale = new Vector2(data.Scale),
                Texture = data.Sprite,
                TextureReference = AssetManager.LoadTexture2D(data.Sprite),
                Layer = drawLayer,
                MinSize = new Vector2(2),
                MaxZoomLevel = 10,
            });

            return entity;

        } // BuildMoon

        public static Entity BuildAsteroid(Registry registry, string id, Vector2I sectorPosition, Entity parent, float orbitStart, float orbit, AsteroidData data, FastRandom rng, int drawLayer)
        {
            var atlasRect = Globals.EntityAtlas.GetSpriteRect(data.Sprite);
            var entity = registry.CreateEntity();

            var orbitLength = 2f * MathF.PI * orbit;

            entity.TryAddComponent(new Asteroid());
            entity.TryAddComponent(new Transform()
            {
                Rotation = 0f,
                Position = GetOrbitPosition(orbit),
                Parent = parent,
            });
            entity.TryAddComponent(new Orbit()
            {
                Parent = parent,
                Speed = rng.Next(20, 100),
                Radius = orbit,
                StartIndex = (int)(orbitLength * orbitStart),
            });
            entity.TryAddComponent(new OrbitalBody()
            {
                ID = id,
                Parent = parent,
                Sector = sectorPosition,
            });
            entity.TryAddComponent(new Drawable()
            {
                AtlasRect = atlasRect,
                Origin = atlasRect.SizeF / 2f,
                Scale = new Vector2(data.Scale),
                Texture = data.Sprite,
                TextureReference = AssetManager.LoadTexture2D(Globals.EntityAtlas.TextureAsset),
                Layer = drawLayer,
                MinSize = new Vector2(1),
                MaxZoomLevel = 10,
            });

            if (parent.HasComponent<Planet>())
                entity.TryAddComponent(new RingAsteroid(parent));

            return entity;

        } // BuildAsteroid

    } // GalaxyPrefabs
}
