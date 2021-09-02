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

        public static void SetSyncEveryTick<T>(Entity entity) where T : struct
        {
            entity.TryAddComponent(new EveryFrameSync<T>());
        }

        public static void SetNeedsTempNetworkSync<T>(Entity entity) where T : struct
        {
            entity.TryAddComponent(new TempSync<T>());
            entity.TryAddComponent(new PlayerJoinedSync<T>());
        }

    } // EntityUtility
}
