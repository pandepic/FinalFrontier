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
        public class SpawnPoint
        {
            public Vector2I Sector;
            public ClassType Class;
            public QualityType Quality;
            public int Level;
            public Entity Target;
            public int WaveSizeMin;
            public int WaveSizeMax;
            public List<Entity> CurrentWave = new List<Entity>();
        }

        public GameServer GameServer;
        public int NextColonyNameIndex = 0;

        public List<Vector2I> ColonisedSectors = new List<Vector2I>();
        public List<Entity> Colonies = new List<Entity>();
        public List<SpawnPoint> SpawnPoints = new List<SpawnPoint>();

        public ServerWorldManager(GameServer gameServer)
        {
            GameServer = gameServer;
        }

        public void Update()
        {
            foreach (var spawnPoint in SpawnPoints)
            {
                for (var i = spawnPoint.CurrentWave.Count - 1; i >= 0; i--)
                {
                    var entity = spawnPoint.CurrentWave[i];

                    if (!entity.IsAlive)
                        spawnPoint.CurrentWave.RemoveAt(i);
                }

                if (spawnPoint.CurrentWave.Count == 0)
                    SpawnAlienWave(Globals.RNG.Next(spawnPoint.WaveSizeMin, spawnPoint.WaveSizeMax + 1), spawnPoint.Target, spawnPoint);
            }

        } // Update

        public void SetupWorld()
        {
            ColonisedSectors.Clear();
            Colonies.Clear();
            SpawnPoints.Clear();

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

            while (NextColonyNameIndex < Globals.ColonyNames.Count)
            {
                Entity closestPotentialColony = new Entity();
                float closestPotentialColonyDistance = 0f;

                foreach (var (sectorPosition, sectorData) in GameServer.GalaxyGenerator.GalaxyStars)
                {
                    foreach (var planet in sectorData.Planets)
                    {
                        if (planet.HasComponent<Colonisable>() && !planet.HasComponent<Colony>())
                        {
                            ref var planetTransform = ref planet.GetComponent<Transform>();

                            if (ColonisedSectors.Contains(planetTransform.TransformedSectorPosition))
                                continue;

                            var distance = homeWorldTransform.TransformedSectorPosition.GetDistance(planetTransform.TransformedSectorPosition);

                            if (closestPotentialColonyDistance == 0 || distance < closestPotentialColonyDistance)
                            {
                                closestPotentialColony = planet;
                                closestPotentialColonyDistance = distance;
                            }
                        }
                    }
                }

                if (!closestPotentialColony.IsAlive)
                    break;

                AddColony(closestPotentialColony);
            }

            foreach (var (sectorPosition, sectorData) in GameServer.GalaxyGenerator.GalaxyStars)
            {
                if (ColonisedSectors.Contains(sectorPosition))
                    continue;

                var distanceFromHomeWorld = ColonisedSectors[0].GetDistance(sectorPosition);
                var level = (int)(distanceFromHomeWorld / 10) + 1;

                var spawnClass = level / 7;
                if (spawnClass > 2)
                    spawnClass = 2;

                var spawnQualityIndex = level % 7;
                var spawnQuality = QualityType.Common;

                switch (spawnQualityIndex)
                {
                    case 0:
                    case 1:
                        spawnQuality = QualityType.Common;
                        break;

                    case 2:
                    case 3:
                        spawnQuality = QualityType.Uncommon;
                        break;

                    case 4:
                    case 5:
                        spawnQuality = QualityType.Rare;
                        break;

                    case 6:
                        spawnQuality = QualityType.Legendary;
                        break;
                }

                var spawnPoint = new SpawnPoint()
                {
                    Sector = sectorPosition,
                    Class = (ClassType)spawnClass,
                    Quality = spawnQuality,
                    Target = sectorData.Planets.GetRandomItem(),
                    Level = level,
                    WaveSizeMin = (level / 5) + 1,
                    WaveSizeMax = (level / 5) + 1,
                };

                SpawnPoints.Add(spawnPoint);
            }
            
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

        public void SpawnAlienWave(int waveSize, Entity target, SpawnPoint spawnPoint)
        {
            Logging.Debug("Spawning {waveSize} aliens at level {level} in sector {spawnSector} with class {class} and quality {quality}",
                waveSize, spawnPoint.Level, spawnPoint.Sector, spawnPoint.Class, spawnPoint.Quality);

            ref var targetTransform = ref target.GetComponent<Transform>();

            var spawnSector = targetTransform.TransformedSectorPosition + Globals.SurroundingPositions.GetRandomItem();
            var spawnOrigin = new Vector2(Globals.GalaxySectorScale / 2);

            targetTransform.Position = EntityUtility.GetOrbitPosition(target, GameServer.NetworkServer.WorldTime);
            var sentrySector = targetTransform.TransformedSectorPosition;

            for (var i = 0; i < waveSize; i++)
            {
                var sentryPosition = targetTransform.TransformedPosition + new Vector2(Globals.RNG.Next(-1000, 1000), Globals.RNG.Next(-1000, 1000));

                var ship = ShipPrefabs.AlienShip(GameServer, spawnPoint.Class, spawnPoint.Quality,
                    spawnOrigin + new Vector2(Globals.RNG.Next(-1000, 1000), Globals.RNG.Next(-1000, 1000)), spawnSector,
                    sentryPosition, sentrySector, spawnPoint.Level);

                spawnPoint.CurrentWave.Add(ship);
            }
        } // SpawnAlienWave

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

            //for (var i = 0; i < 100; i++)
            //    CheckLootDrop(player.User.Username);

        } // SpawnPlayerShip

        public void CheckLootDrop(string username)
        {
            var player = GameServer.NetworkServer.PlayerManager.GetPlayer(username);

            if (player == null || !player.IsPlaying || !player.Ship.IsAlive)
                return;

            ref var inventory = ref player.Ship.GetComponent<Inventory>();

            if (Globals.RNG.Next(0, 100) > 20)
                return;

            if (inventory.Items.Count >= 25)
            {
                GameServer.NetworkServer.SendSystemMessage(player, $"Couldn't pick up loot, inventory is full at 25 items.");
                return;
            }

            var qualityRoll = Globals.RNG.Next(0, 100);
            var quality = QualityType.Common;

            if (qualityRoll > 95)
                quality = QualityType.Legendary;
            else if (qualityRoll > 70)
                quality = QualityType.Rare;
            else if (qualityRoll > 40)
                quality = QualityType.Uncommon;

            var dropTypeRoll = Globals.RNG.Next(0, 4);

            ShipComponentType? dropType = dropTypeRoll switch
            {
                0 => null,
                1 => ShipComponentType.Engine,
                2 => ShipComponentType.Shield,
                3 => ShipComponentType.Armour,
                _ => throw new NotImplementedException(),
            };

            var classTypeRoll = Globals.RNG.Next(0, 3);

            var classType = classTypeRoll switch
            {
                0 => ClassType.Small,
                1 => ClassType.Medium,
                2 => ClassType.Large,
                _ => throw new NotImplementedException(),
            };

            var inventoryItem = new InventoryItem()
            {
                Username = username,
                ComponentType = dropType,
                Seed = Guid.NewGuid().ToString(),
                Quality = quality,
                ClassType = dropType.HasValue ? null : classType,
            };

            using var command = GameServer.NetworkServer.Database.Connection.CreateCommand();
            inventoryItem.Insert(command);
            inventory.Items.Add(inventoryItem);

            var dropTypeText = dropType.HasValue ? dropType.Value.ToString() : "Weapon";

            EntityUtility.SetNeedsTempNetworkSync<Inventory>(player.Ship);
            GameServer.NetworkServer.SendSystemMessage(player, $"Looted a {quality} {dropTypeText}.");

        } // CheckLootDrop

    } // ServerWorldManager
}
