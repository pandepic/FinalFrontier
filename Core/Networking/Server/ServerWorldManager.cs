using ElementEngine;
using ElementEngine.ECS;
using FinalFrontier.Components;
using FinalFrontier.Database.Tables;
using FinalFrontier.Networking;
using FinalFrontier.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier
{
    public class ServerWorldManager
    {
        public GameServer GameServer;
        public int NextColonyNameIndex = 0;

        public List<Vector2I> ColonisedSectors = new List<Vector2I>();
        public List<Entity> Colonies = new List<Entity>();

        public ServerWorldManager(GameServer gameServer)
        {
            GameServer = gameServer;
        }

        public void SetupWorld()
        {
            Entity homeWorld = new Entity();

            foreach (var (sectorPosition, sectorData) in GameServer.GalaxyGenerator.GalaxyStars)
            {
                foreach (var planet in sectorData.Planets)
                {
                    if (planet.HasComponent<Colonisable>())
                    {
                        homeWorld = planet;
                        break;
                    }
                }

                if (homeWorld.IsAlive)
                    break;
            }

            AddColony(homeWorld);

            ref var homeWorldTransform = ref homeWorld.GetComponent<Transform>();

            //while (NextColonyNameIndex < Globals.ColonyNames.Count)
            //{
            //    Entity closestPotentialColony = new Entity();
            //    float closestPotentialColonyDistance = 0f;

            //    foreach (var (sectorPosition, sectorData) in GameServer.GalaxyGenerator.GalaxyStars)
            //    {
            //        foreach (var planet in sectorData.Planets)
            //        {
            //            if (planet.HasComponent<Colonisable>() && !planet.HasComponent<Colony>())
            //            {
            //                ref var planetTransform = ref planet.GetComponent<Transform>();

            //                if (ColonisedSectors.Contains(planetTransform.TransformedSectorPosition))
            //                    continue;

            //                var distance = homeWorldTransform.TransformedSectorPosition.GetDistance(planetTransform.TransformedSectorPosition);

            //                if (closestPotentialColonyDistance == 0 || distance < closestPotentialColonyDistance)
            //                {
            //                    closestPotentialColony = planet;
            //                    closestPotentialColonyDistance = distance;
            //                }
            //            }
            //        }
            //    }

            //    if (!closestPotentialColony.IsAlive)
            //        break;

            //    AddColony(closestPotentialColony);
            //}
        } // SetupWorld

        public void AddColony(Entity planet)
        {
            if (NextColonyNameIndex >= Globals.ColonyNames.Count)
                return;

            var name = Globals.ColonyNames[NextColonyNameIndex++];
            
            planet.TryAddComponent(new Colony() { Name = name });

            planet.TryAddComponent(new WorldSpaceLabel()
            {
                TextSize = 20,
                BaseText = name,
                Text = name,
                Color = Veldrid.RgbaByte.CornflowerBlue,
                TextOutline = 1,
                MarginBottom = 0,
            });

            EntityUtility.SetNeedsTempNetworkSync<Colony>(planet);
            EntityUtility.SetNeedsTempNetworkSync<WorldSpaceLabel>(planet);

            ref var transform = ref planet.GetComponent<Transform>();

            Colonies.Add(planet);
            ColonisedSectors.Add(transform.TransformedSectorPosition);

            Logging.Information("Spawned colony {name} at sector {sector} and position {position}.", name, transform.TransformedSectorPosition, transform.TransformedPosition);
        }

        public void DestroyEntity(NetworkPacket packet, Entity entity)
        {
            GameServer.Registry.DestroyEntity(entity);
            DestroyEntityRequest.Write(packet, entity.ID);
        }

        public void SpawnAlienWave(GameServer gameServer, int waveSize)
        {
            var targetColony = Colonies.GetRandomItem();
            ref var targetColonyTransform = ref targetColony.GetComponent<Transform>();
            ref var colonyComponent = ref targetColony.GetComponent<Colony>();

            var spawnSector = targetColonyTransform.TransformedSectorPosition + Globals.SurroundingPositions.GetRandomItem();
            var spawnOrigin = new Vector2(Globals.GalaxySectorScale / 2);

            targetColonyTransform.Position = EntityUtility.GetOrbitPosition(targetColony, gameServer.NetworkServer.WorldTime);
            var sentrySector = targetColonyTransform.TransformedSectorPosition;

            for (var i = 0; i < waveSize; i++)
            {
                var sentryPosition = targetColonyTransform.TransformedPosition + new Vector2(Globals.RNG.Next(-1000, 1000), Globals.RNG.Next(-1000, 1000));

                ShipPrefabs.AlienShip(GameServer, ClassType.Small, QualityType.Common,
                    spawnOrigin + new Vector2(Globals.RNG.Next(-1000, 1000), Globals.RNG.Next(-1000, 1000)), spawnSector,
                    sentryPosition, sentrySector);
            }

            Logging.Information("Spawned alien wave at {spawnSector} to attack {colony} at {colonySector}", spawnSector, colonyComponent.Name, targetColonyTransform.TransformedSectorPosition);
        }

        public void SpawnPlayerShip(GameServer gameServer, Networking.Database database, Networking.Server.Player player)
        {
            var playerShips = database.GetUserShips(player.User);
            UserShip activeShip = null;

            if (playerShips.Count == 0)
            {
                var shipData = GameDataManager.Ships["Patrol Cutter"];

                activeShip = new UserShip()
                {
                    Username = player.User.Username,
                    ShipName = shipData.Name,
                    IsActive = true,
                    Weapons = new List<UserShipWeapon>(),
                    Components = new List<UserShipComponent>(),
                };

                foreach (var slot in Enum.GetValues<ShipComponentType>())
                {
                    activeShip.Components.Add(new UserShipComponent()
                    {
                        Username = player.User.Username,
                        ShipName = shipData.Name,
                        Slot = slot,
                        Seed = Guid.NewGuid().ToString(),
                        Quality = QualityType.Common,
                    });
                }

                for (int i = 0; i < shipData.Turrets.Count; i++)
                {
                    activeShip.Weapons.Add(new UserShipWeapon()
                    {
                        Username = player.User.Username,
                        ShipName = shipData.Name,
                        Slot = i,
                        Seed = Guid.NewGuid().ToString(),
                        Quality = QualityType.Common,
                    });
                }

                using var command = database.Connection.CreateCommand();
                activeShip.Insert(command);
            }
            else
            {
                foreach (var (_, ship) in playerShips)
                {
                    if (ship.IsActive)
                    {
                        activeShip = ship;
                        break;
                    }
                }
            }

            var homeWorld = Colonies[0];
            ref var homeWorldTransform = ref homeWorld.GetComponent<Transform>();
            homeWorldTransform.Position = EntityUtility.GetOrbitPosition(homeWorld, gameServer.NetworkServer.WorldTime);

            player.Ship = ShipPrefabs.PlayerShip(gameServer, player, activeShip, homeWorldTransform.TransformedPosition, homeWorldTransform.TransformedSectorPosition);
        }

    } // ServerWorldManager
}
