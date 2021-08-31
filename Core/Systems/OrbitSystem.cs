using ElementEngine;
using FinalFrontier.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Systems
{
    public static class OrbitSystem
    {
        public static void Run(GalaxyGenerator galaxyGenerator, Camera2D camera, Vector2I cameraSector, double worldTime)
        {
            var visibleSectors = ClientUtility.GetVisibleSectors(camera, cameraSector);

            foreach (var sectorPos in visibleSectors)
            {
                if (!galaxyGenerator.GalaxyStars.TryGetValue(sectorPos, out var sectorData))
                    continue;

                if (sectorData.Entities == null)
                    continue;

                foreach (var entity in sectorData.Entities)
                {
                    if (!entity.HasComponent<Orbit>())
                        continue;

                    ref var transform = ref entity.GetComponent<Transform>();
                    transform.Position = EntityUtility.GetOrbitPosition(entity, worldTime);
                }
            }
        }

    } // OrbitSystem
}
