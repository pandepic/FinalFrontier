using ElementEngine;
using ElementEngine.ECS;
using FinalFrontier.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier
{
    public static class ProjectileSystem
    {
        private static List<Group> _enemyGroups = new List<Group>();

        public static void Run(GameServer gameServer, Group group, GameTimer gameTimer)
        {
            foreach (var entity in group.Entities)
            {
                ref var transform = ref entity.GetComponent<Transform>();
                ref var projectile = ref entity.GetComponent<Projectile>();

                var entityFullPosition = EntityUtility.GetFullPosition(ref transform);

                _enemyGroups.Clear();

                if (entity.HasComponent<Human>())
                    _enemyGroups.Add(gameServer.AlienGroup);
                else if (entity.HasComponent<Alien>())
                    _enemyGroups.Add(gameServer.HumanGroup);

                foreach (var enemyGroup in _enemyGroups)
                {
                    foreach (var enemyEntity in enemyGroup.Entities)
                    {
                        ref var enemyTransform = ref enemyEntity.GetComponent<Transform>();

                        if (enemyTransform.SectorPosition != transform.SectorPosition)
                            continue;

                        ref var drawable = ref entity.GetComponent<Drawable>();
                        ref var enemyDrawable = ref entity.GetComponent<Drawable>();

                        var projectileAABB = new Rectangle(transform.TransformedPosition.ToVector2I(), (drawable.AtlasRect.Size.ToVector2() * drawable.Scale).ToVector2I());
                        var enemyAABB = new Rectangle(enemyTransform.TransformedPosition.ToVector2I(), (enemyDrawable.AtlasRect.Size.ToVector2() * enemyDrawable.Scale).ToVector2I());

                        if (projectileAABB.Intersects(enemyAABB))
                        {
                            ref var enemyShield = ref enemyEntity.GetComponent<Shield>();
                            ref var enemyArmour = ref enemyEntity.GetComponent<Armour>();

                            var remainingDamage = 0f;

                            if (enemyShield.CurrentValue > 0)
                            {
                                enemyShield.CurrentValue -= projectile.Damage;

                                if (enemyShield.CurrentValue < 0)
                                    remainingDamage = Math.Abs(enemyShield.CurrentValue);
                            }
                            else
                                enemyArmour.CurrentValue -= projectile.Damage;

                            if (remainingDamage > 0)
                                enemyArmour.CurrentValue -= remainingDamage;

                            if (enemyShield.CurrentValue < 0)
                                enemyShield.CurrentValue = 0;

                            if (enemyArmour.CurrentValue < 0)
                            {
                                gameServer.ServerWorldManager.DestroyEntity(gameServer.NetworkServer.NextPacket, enemyEntity);

                                var projectileTurret = projectile.Parent.GetComponent<Turret>();
                                var projectileShip = projectileTurret.Parent;

                                if (enemyEntity.HasComponent<Loot>() && projectileShip.HasComponent<PlayerShip>())
                                {
                                    ref var loot = ref enemyEntity.GetComponent<Loot>();
                                    ref var playerShip = ref projectileShip.GetComponent<PlayerShip>();

                                    gameServer.NetworkServer.PlayerManager.GiveExpMoney(playerShip.Username, loot.Bounty, loot.Exp);
                                    gameServer.ServerWorldManager.CheckLootDrop(playerShip.Username);
                                }
                            }
                            else
                            {
                                EntityUtility.SetNeedsTempNetworkSync<Shield>(enemyEntity);
                                EntityUtility.SetNeedsTempNetworkSync<Armour>(enemyEntity);
                            }

                            gameServer.ServerWorldManager.DestroyEntity(gameServer.NetworkServer.NextPacket, entity);
                            continue;
                        }
                    }
                }

                projectile.Lifetime -= gameTimer.DeltaS;

                if (projectile.Lifetime <= 0)
                {
                    gameServer.ServerWorldManager.DestroyEntity(gameServer.NetworkServer.NextPacket, entity);
                    continue;
                }

                if (entity.HasComponent<Missile>())
                {
                    ref var missile = ref entity.GetComponent<Missile>();
                    ref var physics = ref entity.GetComponent<Physics>();

                    if (!missile.Target.IsAlive)
                        continue;

                    var targetFullPosition = EntityUtility.GetEntityFullPosition(missile.Target);
                    missile.TargetRotation = (float)MathHelper.GetAngleDegreesBetweenPositions(entityFullPosition, targetFullPosition);

                    EntityUtility.HandleRotationTowardsTarget(ref transform, missile.TurnSpeed, missile.TargetRotation, gameTimer.DeltaS);

                    var forwardVector = new Vector2(0f, -1f);
                    var rotaterMatrix = Matrix3x2.CreateRotation(transform.Rotation.ToRadians());
                    forwardVector = Vector2.TransformNormal(forwardVector, rotaterMatrix);
                    physics.Velocity = forwardVector * missile.MoveSpeed;
                }
            }
        }

    } // ProjectileSystem
}
