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
    public static class ShipSystem
    {
        public static void Run(Group shipGroup, GameTimer gameTimer)
        {
            foreach (var entity in shipGroup.Entities)
            {
                ref var physics = ref entity.GetComponent<Physics>();
                Vector2D target;
                bool orbit;

                if (entity.HasComponent<MoveToPosition>())
                {
                    ref var moveToPosition = ref entity.GetComponent<MoveToPosition>();
                    target = EntityUtility.GetFullPosition(moveToPosition.Position, moveToPosition.SectorPosition);
                    orbit = moveToPosition.Orbit;
                }
                else if (entity.HasComponent<MoveToEntity>())
                {
                    ref var moveToEntity = ref entity.GetComponent<MoveToEntity>();

                    if (!moveToEntity.Target.IsAlive)
                    {
                        physics.Velocity = Vector2.Zero;
                        EntityUtility.RemoveMovementComponents(entity);
                        continue;
                    }

                    target = EntityUtility.GetEntityFullPosition(moveToEntity.Target);
                    orbit = moveToEntity.Orbit;
                }
                else
                {
                    continue;
                }

                ref var transform = ref entity.GetComponent<Transform>();
                ref var ship = ref entity.GetComponent<Ship>();
                ref var engine = ref entity.GetComponent<ShipEngine>();

                var entityFullPosition = EntityUtility.GetEntityFullPosition(entity);
                ship.TargetRotation = (float)MathHelper.GetAngleDegreesBetweenPositions(entityFullPosition, target);

                var totalMoveSpeed = ship.MoveSpeed;
                var totalTurnSpeed = ship.TurnSpeed;
                var engineComponent = EntityUtility.GetShipComponent<ShipEngineData>(ShipComponentType.Engine, entity);

                if (engineComponent != null)
                {
                    totalMoveSpeed *= engineComponent.MoveSpeedBonus;
                    totalTurnSpeed *= engineComponent.TurnSpeedBonus;
                }

                var rotated = EntityUtility.HandleRotationTowardsTarget(ref transform, totalTurnSpeed, ship.TargetRotation, gameTimer.DeltaS);
                var distanceToDestination = Vector2D.GetDistance(entityFullPosition, target);

                if (engine.WarpIsActive && distanceToDestination <= Globals.WARP_DRIVE_STOP_DISTANCE)
                {
                    engine.WarpIsActive = false;

                    if (entity.HasComponent<WorldSpaceLabel>())
                    {
                        ref var worldSpaceLabel = ref entity.GetComponent<WorldSpaceLabel>();
                        worldSpaceLabel.Text = worldSpaceLabel.BaseText;
                        EntityUtility.SetNeedsTempNetworkSync<WorldSpaceLabel>(entity);
                    }
                }
                else if (!rotated && !engine.WarpIsActive && distanceToDestination >= Globals.WARP_DRIVE_SECTOR_DISTANCE)
                {
                    engine.WarpIsActive = true;
                    engine.WarpCooldown = engine.BaseWarpCooldown;

                    if (engineComponent != null)
                        engine.WarpCooldown -= engineComponent.WarpCooldownReduction;
                }
                else if (engine.WarpIsActive && engine.WarpCooldown > 0)
                {
                    engine.WarpCooldown -= gameTimer.DeltaS;

                    if (engine.WarpCooldown <= 0)
                        engine.WarpCooldown = 0;

                    if (entity.HasComponent<WorldSpaceLabel>())
                    {
                        ref var worldSpaceLabel = ref entity.GetComponent<WorldSpaceLabel>();
                        worldSpaceLabel.Text = worldSpaceLabel.BaseText + $" [Warp in {engine.WarpCooldown:0.00}]";
                        EntityUtility.SetNeedsTempNetworkSync<WorldSpaceLabel>(entity);
                    }
                }
                else if (engine.WarpIsActive && engine.WarpCooldown == 0)
                {
                    totalMoveSpeed = engine.SectorWarpSpeed;

                    if (distanceToDestination >= Globals.WARP_DRIVE_GALAXY_DISTANCE)
                        totalMoveSpeed = engine.GalaxyWarpSpeed;

                    if (engineComponent != null)
                        totalMoveSpeed *= engineComponent.WarpSpeedBonus;
                }

                var forwardVector = new Vector2(0f, -1f);
                var rotaterMatrix = Matrix3x2.CreateRotation(transform.Rotation.ToRadians());
                forwardVector = Vector2.TransformNormal(forwardVector, rotaterMatrix);

                var currentMoveSpeed = forwardVector * totalMoveSpeed;
                physics.Velocity = currentMoveSpeed;

                var checkDistanceDiff = 25f;
                distanceToDestination = Vector2D.GetDistance(entityFullPosition, target);

                if (entity.HasComponent<MoveToEntity>())
                {
                    ref var moveToEntity = ref entity.GetComponent<MoveToEntity>();
                    checkDistanceDiff = Math.Max(moveToEntity.TargetDistance, checkDistanceDiff);
                }

                if (!orbit && distanceToDestination <= checkDistanceDiff)
                {
                    physics.Velocity = Vector2.Zero;
                    EntityUtility.RemoveMovementComponents(entity);
                }
            }
        } // Run

    } // ShipSystem
}
