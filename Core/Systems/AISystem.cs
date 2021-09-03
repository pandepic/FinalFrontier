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
    public static class AISystem
    {
        private static List<Group> _enemyGroups = new List<Group>();
        private static List<Entity> _possibleTargetList = new List<Entity>();

        public static void RunAlien(GameServer gameServer, Group group)
        {
            _enemyGroups.Clear();
            _enemyGroups.Add(gameServer.HumanGroup);

            foreach (var entity in group.Entities)
            {
                ref var transform = ref entity.GetComponent<Transform>();
                ref var aiSentry = ref entity.GetComponent<AISentry>();

                var entityFullPosition = EntityUtility.GetEntityFullPosition(entity);
                var sentryTargetFullPosition = EntityUtility.GetFullPosition(aiSentry.SentryPosition, aiSentry.Sector);
                var distanceFromSentryTarget = Vector2D.GetDistance(entityFullPosition, sentryTargetFullPosition);

                if (distanceFromSentryTarget >= aiSentry.MaxEnemyChaseRange)
                {
                    if (entity.HasComponent<MoveToEntity>())
                        entity.TryRemoveComponentImmediate<MoveToEntity>();

                    if (!EntityUtility.IsMoving(entity))
                    {
                        entity.TryAddComponent(new MoveToPosition()
                        {
                            Position = aiSentry.SentryPosition,
                            SectorPosition = aiSentry.Sector,
                            Orbit = false,
                        });
                    }
                }
                else
                {
                    if (EntityUtility.IsMoving(entity))
                        continue;

                    _possibleTargetList.Clear();

                    foreach (var enemyGroup in _enemyGroups)
                    {
                        foreach (var enemyEntity in enemyGroup.Entities)
                        {
                            ref var enemyTransform = ref enemyEntity.GetComponent<Transform>();

                            if (enemyTransform.SectorPosition != aiSentry.Sector)
                                continue;

                            var distanceFromEnemy = aiSentry.SentryPosition.GetDistance(enemyTransform.Position);

                            if (distanceFromEnemy >= aiSentry.MaxEnemySearchRange)
                                continue;

                            _possibleTargetList.Add(enemyEntity);
                        }
                    }

                    if (_possibleTargetList.Count == 0)
                        continue;

                    var enemyTarget = _possibleTargetList.GetRandomItem();
                    entity.TryAddComponent(new MoveToEntity()
                    {
                        Target = enemyTarget,
                        TargetDistance = 500f,
                        Orbit = true,
                    });
                }
            }

        } // RunAlien

    } // AISystem
}
