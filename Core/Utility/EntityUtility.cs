using ElementEngine;
using ElementEngine.ECS;
using FinalFrontier.Components;
using FinalFrontier.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier
{
    public static class EntityUtility
    {
        public static Vector2 GetOrbitPosition(Entity entity, double worldTime)
        {
            ref var orbit = ref entity.GetComponent<Orbit>();

            var orbitLength = 2f * MathF.PI * orbit.Radius; // in world space pixels
            var orbitTime = orbitLength / orbit.Speed; // in seconds

            var currentRotationProgress = worldTime % orbitTime;
            var currentRotationPercentage = currentRotationProgress / orbitTime;
            var currentRotationIndex = currentRotationPercentage * orbitLength;

            currentRotationIndex += orbit.StartIndex;

            if (currentRotationIndex >= orbitLength)
                currentRotationIndex -= orbitLength;

            return MathHelper.GetPointOnCircle(Vector2.Zero, orbit.Radius, (int)currentRotationIndex, (int)orbitLength);

        } // GetOrbitPosition

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2D GetFullPosition(Vector2 position, Vector2I sector)
        {
            return new Vector2D(
                (sector.X * Globals.GalaxySectorScale) + position.X,
                (sector.Y * Globals.GalaxySectorScale) + position.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2D GetEntityFullPosition(Entity entity)
        {
            ref var transform = ref entity.GetComponent<Transform>();

            return new Vector2D(
                (transform.TransformedSectorPosition.X * Globals.GalaxySectorScale) + transform.TransformedPosition.X,
                (transform.TransformedSectorPosition.Y * Globals.GalaxySectorScale) + transform.TransformedPosition.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle GetEntityRect(Entity entity)
        {
            ref var transform = ref entity.GetComponent<Transform>();
            ref var drawable = ref entity.GetComponent<Drawable>();

            return GetEntityRect(ref transform, ref drawable);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle GetEntityRect(ref Transform transform, ref Drawable drawable)
        {
            return new Rectangle(transform.TransformedPosition - (drawable.Origin * drawable.Scale), drawable.AtlasRect.SizeF * drawable.Scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMoving(Entity entity)
        {
            return entity.HasComponent<MoveToEntity>() || entity.HasComponent<MoveToPosition>();
        }

        public static void SetSyncEveryTick<T>(Entity entity) where T : struct
        {
            entity.TryAddComponent(new EveryFrameSync<T>());
        }

        public static void SetNeedsTempNetworkSync<T>(Entity entity) where T : struct
        {
            entity.TryAddComponent(new TempSync<T>());
            entity.TryAddComponent(new PlayerJoinedSync<T>());
        }

        public static void RemoveMovementComponents(Entity entity)
        {
            if (entity.HasComponent<MoveToPosition>())
                entity.RemoveComponent<MoveToPosition>();
            else if (entity.HasComponent<MoveToEntity>())
                entity.RemoveComponent<MoveToEntity>();
        }

        public static void ImmediateRemoveMovementComponents(Entity entity)
        {
            if (entity.HasComponent<MoveToPosition>())
                entity.TryRemoveComponentImmediate<MoveToPosition>();
            else if (entity.HasComponent<MoveToEntity>())
                entity.TryRemoveComponentImmediate<MoveToEntity>();
        }

        public static T GetShipComponent<T>(ShipComponentType type, Entity ship) where T : ShipComponentData
        {
            ref var shipComponent = ref ship.GetComponent<Ship>();

            if (!shipComponent.ShipComponentData.TryGetValue(type, out var componentSlot))
                return null;

            return componentSlot.ComponentData as T;
        }

    } // EntityUtility
}
