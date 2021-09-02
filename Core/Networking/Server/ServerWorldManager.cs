using ElementEngine;
using ElementEngine.ECS;
using FinalFrontier.Components;
using FinalFrontier.Database.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier
{
    public class ServerWorldManager
    {
        public GameServer GameServer;
        public int NextColonyNameIndex = 0;

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
        }

        public void AddColony(Entity planet)
        {
            if (NextColonyNameIndex >= Globals.ColonyNames.Count)
                return;

            var name = Globals.ColonyNames[NextColonyNameIndex++];
            planet.TryAddComponent(new Colony() { Name = name });
            EntityUtility.SetNeedsTempNetworkSync<Colony>(planet);

            ref var transform = ref planet.GetComponent<Transform>();

            Colonies.Add(planet);
            Logging.Information("Spawned colony {name} at sector {sector} and position {position}.", name, transform.TransformedSectorPosition, transform.TransformedPosition);
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
                        Quality = ComponentQualityType.Common,
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
                        Quality = ComponentQualityType.Common,
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

            player.Ship = ShipPrefabs.CreatePlayerShip(gameServer, player, activeShip, homeWorldTransform.TransformedPosition, homeWorldTransform.TransformedSectorPosition);
        }

    } // ServerWorldManager
}
