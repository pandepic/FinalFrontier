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
        private static SparseSet<Entity> _worldSpaceLabelEntities = new SparseSet<Entity>(1000);
        private static SparseSet<Entity> _worldIconEntities = new SparseSet<Entity>(1000);

        public static void BuildDrawList(int zoomLevel, List<Vector2I> visibleSectors, SparseSet<Entity> entities, Group drawableGroup, GalaxyGenerator galaxyGenerator)
        {
            _worldSpaceLabelEntities.Clear();

            foreach (var sectorPos in visibleSectors)
            {
                if (!galaxyGenerator.GalaxyStars.TryGetValue(sectorPos, out var sectorData))
                    continue;

                entities.TryAdd(sectorData.Star, out var _);

                AddOrbitalBodiesToDrawList(sectorData.Planets, zoomLevel, entities);

                if (zoomLevel <= Globals.MAX_ZOOM_MOON)
                    AddOrbitalBodiesToDrawList(sectorData.Planets, zoomLevel, entities);
                if (zoomLevel <= Globals.MAX_ZOOM_ASTEROID)
                    AddOrbitalBodiesToDrawList(sectorData.Asteroids, zoomLevel, entities);

                //foreach (var entity in sectorData.Entities)
                //{
                //    ref var drawable = ref entity.GetComponent<Drawable>();

                //    if (entity.HasComponent<WorldSpaceLabel>())
                //        _worldSpaceLabelEntities.TryAdd(entity, out var _);

                //    if (drawable.MaxZoomLevel == 0 || zoomLevel <= drawable.MaxZoomLevel)
                //        entities.TryAdd(entity, out var _);
                //}
            }

            foreach (var entity in drawableGroup.Entities)
            {
                ref var transform = ref entity.GetComponent<Transform>();

                if (visibleSectors.Contains(transform.TransformedSectorPosition))
                    entities.TryAdd(entity, out var _);
            }
        }

        private static void AddOrbitalBodiesToDrawList(List<Entity> orbitalBodies, int zoomLevel, SparseSet<Entity> entities)
        {
            foreach (var entity in orbitalBodies)
            {
                ref var drawable = ref entity.GetComponent<Drawable>();

                if (entity.HasComponent<WorldSpaceLabel>())
                    _worldSpaceLabelEntities.TryAdd(entity, out var _);

                if (drawable.MaxZoomLevel == 0 || zoomLevel <= drawable.MaxZoomLevel)
                    entities.TryAdd(entity, out var _);
            }
        }
        
        public static unsafe void RunDrawables(SparseSet<Entity> entities, SpriteBatch2D spriteBatch, Camera2D camera, Vector2I cameraSector, int zoomLevel)
        {
            var cameraView = camera.ScaledView;

            _drawList.Clear();
            _worldIconEntities.Clear();

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
                    {
                        _worldIconEntities.TryAdd(entity, out var _);

                        if (zoomLevel <= Globals.MAX_LABEL_ZOOM_LEVEL && entity.HasComponent<WorldSpaceLabel>())
                            _worldSpaceLabelEntities.TryAdd(entity, out var _);
                    }

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
                    Color = drawable.Color.ToRgbaFloat(),
                });

                if (entity.HasComponent<WorldSpaceLabel>())
                    _worldSpaceLabelEntities.TryAdd(entity, out var _);
            }

            EndDrawList(spriteBatch);

        } // RunDrawables

        public static void RunWorldIcons(SpriteBatch2D spriteBatch, Camera2D camera, Vector2I cameraSector)
        {
            _drawList.Clear();

            foreach (var entity in _worldIconEntities)
            {
                ref var transform = ref entity.GetComponent<Transform>();
                ref var worldIcon = ref entity.GetComponent<WorldIcon>();

                var drawPosition = ((transform.TransformedSectorPosition - cameraSector).ToVector2() * Globals.GalaxySectorScale) + transform.TransformedPosition;
                drawPosition = camera.WorldToScreen(drawPosition);

                _drawList.Add(new DrawItem()
                {
                    Position = drawPosition,
                    Origin = worldIcon.Origin,
                    Scale = worldIcon.Scale,
                    Rotation = transform.Rotation,
                    SourceRect = worldIcon.AtlasRect,
                    Texture = worldIcon.TextureReference,
                    Layer = worldIcon.Layer,
                    Color = RgbaFloat.White,
                });
            }

            EndDrawList(spriteBatch);

        } // RunWorldIcons

        public static void RunWorldSpaceLabels(SpriteBatch2D spriteBatch, Camera2D camera, Vector2I cameraSector, SpriteFont font)
        {
            var cameraView = camera.ScaledView;
            _drawListText.Clear();

            foreach (var entity in _worldSpaceLabelEntities)
            {
                ref var transform = ref entity.GetComponent<Transform>();

                ref var drawable = ref entity.GetComponent<Drawable>();
                ref var worldSpaceLabel = ref entity.GetComponent<WorldSpaceLabel>();

                var drawPosition = ((transform.TransformedSectorPosition - cameraSector).ToVector2() * Globals.GalaxySectorScale) + transform.TransformedPosition;
                var entityRect = new Rectangle(drawPosition.ToVector2I() - drawable.Origin.ToVector2I(), drawable.AtlasRect.Size);

                if (!entityRect.Intersects(cameraView))
                    continue;

                var textSize = font.MeasureText(worldSpaceLabel.Text, worldSpaceLabel.TextSize, worldSpaceLabel.TextOutline);
                var screenRect = camera.WorldToScreen(entityRect);
                var textPosition = screenRect.Location + new Vector2I(screenRect.Width / 2, 0) - new Vector2I(textSize.X / 2, textSize.Y + worldSpaceLabel.MarginBottom);

                _drawListText.Add(new DrawItemText()
                {
                    Position = textPosition.ToVector2(),
                    Origin = Vector2.Zero,
                    Scale = new Vector2(1f),
                    Rotation = 0f,
                    Layer = drawable.Layer,
                    Color = worldSpaceLabel.Color,
                    Size = worldSpaceLabel.TextSize,
                    Outline = worldSpaceLabel.TextOutline,
                    Text = worldSpaceLabel.Text,
                });
            }

            if (_drawListText.Count == 0)
                return;

            _drawListText.Sort((x, y) =>
            {
                var val = x.Layer.CompareTo(y.Layer);

                if (val == 0)
                    val = x.Size.CompareTo(y.Size);

                if (val == 0)
                    val = x.Position.Y.CompareTo(y.Position.Y);

                return val;
            });

            foreach (var item in _drawListText)
                spriteBatch.DrawText(font, item.Text, item.Position, item.Color, item.Size, item.Outline);

        } // RunWorldSpaceLabels

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
