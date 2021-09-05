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
    public static class TurretSystem
    {
        private static List<Group> _enemyGroups = new List<Group>();
        private static List<Entity> _possibleTargetListHighPriority = new List<Entity>();
        private static List<Entity> _possibleTargetListLowPriority = new List<Entity>();

        public static void Run(GameServer gameServer, Group group, GameTimer gameTimer)
        {
            foreach (var entity in group.Entities)
            {
                ref var turret = ref entity.GetComponent<Turret>();

                if (!turret.Parent.IsAlive)
                {
                    gameServer.ServerWorldManager.DestroyEntity(gameServer.NetworkServer.NextPacket, entity);
                    continue;
                }

                ref var transform = ref entity.GetComponent<Transform>();
                ref var parentTransform = ref turret.Parent.GetComponent<Transform>();

                var entityFullPosition = EntityUtility.GetEntityFullPosition(entity);

                EntityUtility.HandleRotationTowardsTarget(ref transform, turret.WeaponData.TurretTurnRate, turret.TargetRotation, gameTimer.DeltaS);

                if (turret.Target.IsAlive)
                {
                    var targetFullPosition = EntityUtility.GetEntityFullPosition(turret.Target);

                    turret.TargetRotation = (float)MathHelper.GetAngleDegreesBetweenPositions(entityFullPosition, targetFullPosition);

                    if (entityFullPosition.GetDistance(targetFullPosition) > turret.WeaponData.Range)
                    {
                        turret.Target = new Entity();
                        continue;
                    }

                    turret.CurrentCooldown -= gameTimer.DeltaS;

                    if (turret.CurrentCooldown <= 0)
                    {
                        var angle = MathF.Abs((float)MathHelper.GetAngleDegreesBetweenPositions(entityFullPosition, targetFullPosition) - transform.Rotation);

                        if (angle <= turret.WeaponData.MaxFiringAngle)
                        {
                            if (turret.Parent.HasComponent<ShipEngine>())
                            {
                                ref var shipEngine = ref turret.Parent.GetComponent<ShipEngine>();
                                if (shipEngine.WarpIsActive)
                                    continue;
                            }

                            TurretPrefabs.TurretProjectile(gameServer, entity, transform.Rotation, turret.WeaponData, turret.Target, turret.Parent);
                            turret.CurrentCooldown = turret.WeaponData.Cooldown;
                        }
                    }
                }
                else // has no target
                {
                    turret.CurrentCooldown = 0;
                    turret.TargetRotation = parentTransform.Rotation;

                    _enemyGroups.Clear();

                    if (turret.Parent.HasComponent<Human>())
                        _enemyGroups.Add(gameServer.AlienGroup);
                    else if (turret.Parent.HasComponent<Alien>())
                        _enemyGroups.Add(gameServer.HumanGroup);

                    _possibleTargetListHighPriority.Clear();
                    _possibleTargetListLowPriority.Clear();

                    foreach (var enemyGroup in _enemyGroups)
                    {
                        foreach (var enemyEntity in enemyGroup.Entities)
                        {
                            ref var enemyShip = ref enemyEntity.GetComponent<Ship>();

                            var enemyFullPosition = EntityUtility.GetEntityFullPosition(enemyEntity);
                            var distanceToEnemy = entityFullPosition.GetDistance(enemyFullPosition);

                            if (distanceToEnemy > turret.WeaponData.Range)
                                continue;

                            if (enemyShip.ShipClass == turret.WeaponData.TurretData.Class)
                                _possibleTargetListHighPriority.Add(enemyEntity);
                            else
                                _possibleTargetListLowPriority.Add(enemyEntity);
                        }
                    }

                    var targetList = _possibleTargetListHighPriority;
                    if (targetList.Count == 0)
                        targetList = _possibleTargetListLowPriority;
                    if (targetList.Count == 0)
                        continue;

                    turret.Target = targetList.GetRandomItem();
                }
            }

        } // Run

    } // TurretSystem
}
