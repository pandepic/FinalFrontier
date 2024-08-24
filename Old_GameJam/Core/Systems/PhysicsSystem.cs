using ElementEngine;
using ElementEngine.ECS;
using FinalFrontier.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier
{
    public static class PhysicsSystem
    {
        public static void Run(Group group, GameTimer gameTimer)
        {
            foreach (var entity in group.Entities)
            {
                ref var transform = ref entity.GetComponent<Transform>();
                ref var physics = ref entity.GetComponent<Physics>();

                transform.Position += physics.Velocity * gameTimer.DeltaS;

                // update entity current sector if it has no transform parent
                if (!transform.Parent.IsAlive)
                {
                    var entityRect = EntityUtility.GetEntityRect(entity);

                    if (entityRect.Center.X < 0)
                    {
                        transform.SectorPosition.X -= 1;
                        transform.Position.X += Globals.GalaxySectorScale;
                    }
                    else if (entityRect.Center.X >= Globals.GalaxySectorScale)
                    {
                        transform.SectorPosition.X += 1;
                        transform.Position.X -= Globals.GalaxySectorScale;
                    }

                    if (entityRect.Center.Y < 0)
                    {
                        transform.SectorPosition.Y -= 1;
                        transform.Position.Y += Globals.GalaxySectorScale;
                    }
                    else if (entityRect.Center.Y >= Globals.GalaxySectorScale)
                    {
                        transform.SectorPosition.Y += 1;
                        transform.Position.Y -= Globals.GalaxySectorScale;
                    }
                }
            }
        } // Run

    } // PhysicsSystem
}
