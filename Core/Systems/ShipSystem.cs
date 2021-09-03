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
        public static void RunMovement(Group shipGroup, GameTimer gameTimer)
        {
            foreach (var entity in shipGroup.Entities)
            {
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

                var rotated = HandleRotationTowardsTarget(ref transform, totalTurnSpeed, ship.TargetRotation, gameTimer.DeltaS);
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
                }
                else if (engine.WarpIsActive && engine.WarpCooldown > 0)
                {
                    engine.WarpCooldown -= gameTimer.DeltaS;

                    if (entity.HasComponent<WorldSpaceLabel>())
                    {
                        ref var worldSpaceLabel = ref entity.GetComponent<WorldSpaceLabel>();
                        worldSpaceLabel.Text = worldSpaceLabel.BaseText + $" [Warp in {engine.WarpCooldown:0.00}]";
                        EntityUtility.SetNeedsTempNetworkSync<WorldSpaceLabel>(entity);
                    }

                    if (engine.WarpCooldown <= 0)
                        engine.WarpCooldown = 0;
                }
                else if (engine.WarpIsActive && engine.WarpCooldown == 0)
                {
                    totalMoveSpeed = engine.SectorWarpSpeed;

                    if (distanceToDestination >= Globals.WARP_DRIVE_GALAXY_DISTANCE)
                        totalMoveSpeed = engine.GalaxyWarpSpeed;
                }

                var forwardVector = new Vector2(0f, -1f);
                var rotaterMatrix = Matrix3x2.CreateRotation(transform.Rotation.ToRadians());
                forwardVector = Vector2.TransformNormal(forwardVector, rotaterMatrix);
                
                var currentMoveSpeed = forwardVector * totalMoveSpeed;
                transform.Position += currentMoveSpeed * gameTimer.DeltaS;

                var newEntityFullPosition = EntityUtility.GetEntityFullPosition(entity);
                var movedDistance = Vector2D.GetDistance(newEntityFullPosition, target);

                var checkDistanceDiff = 10f; //Math.Max(50d, movedDistance * 2);
                distanceToDestination = Vector2D.GetDistance(entityFullPosition, target);

                if (!orbit && distanceToDestination <= checkDistanceDiff)
                    EntityUtility.RemoveMovementComponents(entity);

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
        } // RunMovement

        private static bool HandleRotationTowardsTarget(ref Transform transform, float turnSpeed, float targetRotation, float delta)
        {
            if (transform.Rotation == targetRotation)
                return false;

            var absRotDiff = Math.Abs(transform.Rotation - targetRotation);

            if (absRotDiff < 1f)
                return false;

            if (transform.Rotation < targetRotation)
            {
                if (absRotDiff < 180.0f)
                    transform.Rotation += turnSpeed * delta;
                else
                    transform.Rotation -= turnSpeed * delta;
            }
            else
            {
                if (absRotDiff < 180.0f)
                    transform.Rotation -= turnSpeed * delta;
                else
                    transform.Rotation += turnSpeed * delta;
            }

            if (transform.Rotation < 0.0f)
                transform.Rotation += 360.0f;
            else if (transform.Rotation > 360.0f)
                transform.Rotation -= 360.0f;

            return true;

        } // HandleRotationTowardsTarget

    } // ShipSystem
}
