using ElementEngine;
using ElementEngine.ECS;
using FinalFrontier.Components;
using FinalFrontier.GameData;
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

        public static Entity Star(Registry registry, string id, Vector2I sectorPosition, Vector2 position, StarData data, int drawLayer, bool serverMode)
        {
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

            if (!serverMode)
            {
                var texture = AssetManager.Instance.LoadTexture2D(data.Sprite);

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
                    Color = Veldrid.RgbaByte.White,
                });
            }

            return entity;

        } // Star

        public static Entity Planet(Registry registry, string id, Vector2I sectorPosition, Entity parent, float orbit, PlanetData data, Random rng, int drawLayer, bool serverMode)
        {
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

            if (data.IsColonisable)
                entity.TryAddComponent(new Colonisable());

            if (!serverMode)
            {
                var texture = AssetManager.Instance.LoadTexture2D(data.Sprite);

                entity.TryAddComponent(new Drawable()
                {
                    AtlasRect = new Rectangle(Vector2I.Zero, texture.Size),
                    Origin = texture.SizeF / 2f,
                    Scale = new Vector2(data.Scale),
                    Texture = data.Sprite,
                    TextureReference = AssetManager.Instance.LoadTexture2D(data.Sprite),
                    Layer = drawLayer,
                    MinSize = new Vector2(2),
                    MaxZoomLevel = Globals.MAX_ZOOM_PLANET,
                    Color = Veldrid.RgbaByte.White,
                });
            }

            return entity;

        } // Planet

        public static Entity Moon(Registry registry, string id, Vector2I sectorPosition, Entity parent, float orbit, MoonData data, Random rng, int drawLayer, bool serverMode)
        {
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

            if (!serverMode)
            {
                var texture = AssetManager.Instance.LoadTexture2D(data.Sprite);

                entity.TryAddComponent(new Drawable()
                {
                    AtlasRect = new Rectangle(Vector2I.Zero, texture.Size),
                    Origin = texture.SizeF / 2f,
                    Scale = new Vector2(data.Scale),
                    Texture = data.Sprite,
                    TextureReference = AssetManager.Instance.LoadTexture2D(data.Sprite),
                    Layer = drawLayer,
                    MinSize = new Vector2(2),
                    MaxZoomLevel = Globals.MAX_ZOOM_MOON,
                    Color = Veldrid.RgbaByte.White,
                });
            }

            return entity;

        } // BuildMoon

        public static Entity Asteroid(Registry registry, string id, Vector2I sectorPosition, Entity parent, float orbitStart, float orbit, AsteroidData data, Random rng, int drawLayer, bool serverMode)
        {
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

            if (!serverMode)
            {
                var atlasRect = Globals.EntityAtlas.GetSpriteRect(data.Sprite);

                entity.TryAddComponent(new Drawable()
                {
                    AtlasRect = atlasRect,
                    Origin = atlasRect.SizeF / 2f,
                    Scale = new Vector2(data.Scale),
                    Texture = data.Sprite,
                    TextureReference = AssetManager.Instance.LoadTexture2D(Globals.EntityAtlas.TextureAsset),
                    Layer = drawLayer,
                    MinSize = new Vector2(1),
                    MaxZoomLevel = Globals.MAX_ZOOM_ASTEROID,
                    Color = Veldrid.RgbaByte.White,
                });
            }

            if (parent.HasComponent<Planet>())
                entity.TryAddComponent(new RingAsteroid(parent));

            return entity;

        } // Asteroid

    } // GalaxyPrefabs
}
