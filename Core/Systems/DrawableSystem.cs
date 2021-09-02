using ElementEngine;
using ElementEngine.ECS;
using FinalFrontier.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

using Rectangle = ElementEngine.Rectangle;

namespace FinalFrontier
{
    public static class DrawableSystem
    {
        private struct DrawItem
        {
            public Vector2 Position;
            public Vector2 Origin;
            public Vector2 Scale;
            public float Rotation;
            public Rectangle SourceRect;
            public Texture2D Texture;
            public int Layer;
            public RgbaFloat Color;
        }

        private struct DrawItemText
        {
            public Vector2 Position;
            public Vector2 Origin;
            public Vector2 Scale;
            public float Rotation;
            public int Layer;
            public RgbaByte Color;
            public int Size;
            public int Outline;
            public string Text;
        }

        private static List<DrawItem> _drawList = new List<DrawItem>();
        private static List<DrawItemText> _drawListText = new List<DrawItemText>();
        private static List<Entity> _iconDrawList = new List<Entity>();

        public static void BuildDrawList(int zoomLevel, List<Vector2I> visibleSectors, SparseSet<Entity> entities, Group drawableGroup, GalaxyGenerator galaxyGenerator)
        {
            foreach (var sectorPos in visibleSectors)
            {
                if (!galaxyGenerator.GalaxyStars.TryGetValue(sectorPos, out var sectorData))
                    continue;

                foreach (var entity in sectorData.Entities)
                {
                    ref var drawable = ref entity.GetComponent<Drawable>();

                    if (drawable.MaxZoomLevel == 0 || zoomLevel <= drawable.MaxZoomLevel)
                        entities.TryAdd(entity, out var _);
                }
            }

            foreach (var entity in drawableGroup.Entities)
            {
                ref var transform = ref entity.GetComponent<Transform>();

                if (visibleSectors.Contains(transform.TransformedSectorPosition))
                    entities.TryAdd(entity, out var _);
            }
        }
        
        public static unsafe void RunDrawables(SparseSet<Entity> entities, SpriteBatch2D spriteBatch, Camera2D camera, Vector2I cameraSector)
        {
            var cameraView = camera.ScaledView;

            _drawList.Clear();
            _iconDrawList.Clear();

            foreach (var entity in entities)
            {
                ref var transform = ref entity.GetComponent<Transform>();
                ref var drawable = ref entity.GetComponent<Drawable>();

                var drawPosition = ((transform.TransformedSectorPosition - cameraSector).ToVector2() * Globals.GalaxySectorScale) + transform.TransformedPosition;
                var entityRect = EntityUtility.GetEntityRect(ref transform, ref drawable);

                if (!entityRect.Intersects(cameraView))
                    continue;

                var hasIcon = entity.HasComponent<WorldIcon>();

                var lowDetailSize = hasIcon ? new Vector2(5f) : new Vector2(1f);

                var entitySize = entityRect.SizeF * camera.Zoom;
                var lowDetail = (entitySize.X < lowDetailSize.X || entitySize.Y < lowDetailSize.Y);

                if (lowDetail && drawable.MinSize == Vector2.Zero)
                {
                    if (hasIcon)
                        _iconDrawList.Add(entity);

                    continue;
                }

                var scale = drawable.Scale;

                if (drawable.MinSize != Vector2.Zero)
                {
                    if (entitySize.X < drawable.MinSize.X && entitySize.Y < drawable.MinSize.Y)
                        scale *= drawable.MinSize / entitySize;
                }

                _drawList.Add(new DrawItem()
                {
                    Position = drawPosition,
                    Origin = drawable.Origin,
                    Scale = scale,
                    Rotation = transform.Rotation,
                    SourceRect = drawable.AtlasRect,
                    Texture = drawable.TextureReference,
                    Layer = drawable.Layer,
                    Color = RgbaFloat.White,
                });
            }

            EndDrawList(spriteBatch);

        } // RunDrawables

        private static void EndDrawList(SpriteBatch2D spriteBatch)
        {
            if (_drawList.Count == 0)
                return;

            // sort by layer then Y position
            _drawList.Sort((x, y) =>
            {
                var val = x.Layer.CompareTo(y.Layer);

                if (val == 0)
                    val = x.Position.Y.CompareTo(y.Position.Y);

                return val;
            });

            foreach (var item in _drawList)
            {
                spriteBatch.DrawTexture2D(item.Texture, item.Position, sourceRect: item.SourceRect, scale: item.Scale, origin: item.Origin, rotation: item.Rotation, color: item.Color);
            }
        } // EndDrawList

    } // DrawableSystem
}
