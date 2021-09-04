using ElementEngine;
using ElementEngine.ECS;
using FinalFrontier.Components;
using FinalFrontier.Database.Tables;
using FinalFrontier.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier
{
    public static class ShipPrefabs
    {
        public struct ShipPrefabComponent
        {
            public ShipComponentType Slot;
            public string Seed;
            public QualityType Quality;

            public ShipPrefabComponent(UserShipComponent component)
            {
                Slot = component.Slot;
                Seed = component.Seed;
                Quality = component.Quality;
            }
        }

        public struct ShipPrefabWeapon
        {
            public int Slot;
            public string Seed;
            public QualityType Quality;

            public ShipPrefabWeapon(UserShipWeapon weapon)
            {
                Slot = weapon.Slot;
                Seed = weapon.Seed;
                Quality = weapon.Quality;
            }
        }

        private static int GetShipLayer(Entity ship)
        {
            return 200000 + (ship.ID * 1000);
        }

        private static Entity Ship(
            GameServer gameServer,
            string shipName,
            Vector2 spawnPosition,
            Vector2I spawnSector,
            List<ShipPrefabComponent> components,
            List<ShipPrefabWeapon> weapons)
        {
            var ship = gameServer.Registry.CreateEntity();

            var shipData = GameDataManager.Ships[shipName];
            var shipSprite = gameServer.SpriteAtlasData.GetSpriteRect(shipData.Sprite);

            var layer = GetShipLayer(ship);

            ship.TryAddComponent(new Transform()
            {
                Rotation = 0f,
                Position = spawnPosition,
                SectorPosition = spawnSector,
            });

            ship.TryAddComponent(new Physics());

            ship.TryAddComponent(new Drawable()
            {
                AtlasRect = shipSprite,
                Texture = Globals.EntityAtlasTexture,
                Layer = layer,
                Scale = new Vector2(shipData.Scale),
                Color = Veldrid.RgbaByte.White,
            });

            var shipComponent = new Ship()
            {
                ShipType = shipData.Name,
                MoveSpeed = shipData.BaseMoveSpeed,
                TurnSpeed = shipData.BaseTurnSpeed,
                ShipComponentData = new Dictionary<ShipComponentType, ShipComponentSlotData>(),
                ShipWeaponData = new Dictionary<int, ShipWeaponSlotData>(),
            };

            foreach (var component in components)
            {
                var componentSlotData = new ShipComponentSlotData()
                {
                    Slot = component.Slot,
                    Seed = component.Seed,
                    Quality = component.Quality,
                };

                switch (component.Slot)
                {
                    case ShipComponentType.Engine:
                        componentSlotData.ComponentData = new ShipEngineData(componentSlotData);
                        break;
                }

                shipComponent.ShipComponentData.Add(component.Slot, componentSlotData);
            }

            var turretLayer = layer + 1;

            foreach (var weapon in weapons)
            {
                var slotData = new ShipWeaponSlotData()
                {
                    Slot = weapon.Slot,
                    Seed = weapon.Seed,
                    Quality = weapon.Quality,
                };

                shipComponent.ShipWeaponData.Add(weapon.Slot, slotData);
                TurretPrefabs.ShipTurret(gameServer, ship, turretLayer, slotData, shipData);
                turretLayer += 1;
            }

            ship.TryAddComponent(shipComponent);

            ship.TryAddComponent(new ShipEngine()
            {
                BaseWarpCooldown = Globals.BASE_WARP_COOLDOWN,
                SectorWarpSpeed = Globals.BASE_SECTOR_WARP_SPEED,
                GalaxyWarpSpeed = Globals.BASE_GALAXY_WARP_SPEED,
                WarpCooldown = 0f,
                WarpIsActive = false,
            });

            ship.TryAddComponent(new Shield()
            {
                BaseValue = shipData.BaseShield,
                CurrentValue = shipData.BaseShield,
                RechargeRate = shipData.BaseShieldRegen,
            });

            ship.TryAddComponent(new Armour()
            {
                BaseValue = shipData.BaseArmour,
                CurrentValue = shipData.BaseArmour,
            });

            EntityUtility.SetNeedsTempNetworkSync<Transform>(ship);
            EntityUtility.SetNeedsTempNetworkSync<Drawable>(ship);
            EntityUtility.SetNeedsTempNetworkSync<Ship>(ship);
            EntityUtility.SetNeedsTempNetworkSync<Shield>(ship);
            EntityUtility.SetNeedsTempNetworkSync<Armour>(ship);

            EntityUtility.SetSyncEveryTick<Transform>(ship);

            return ship;
        }

        public static Entity PlayerShip(GameServer gameServer, Networking.Server.Player player, UserShip dbShip, Vector2 spawnPosition, Vector2I spawnSector)
        {
            var components = new List<ShipPrefabComponent>();
            var weapons = new List<ShipPrefabWeapon>();

            foreach (var component in dbShip.Components)
                components.Add(new ShipPrefabComponent(component));

            foreach (var weapon in dbShip.Weapons)
                weapons.Add(new ShipPrefabWeapon(weapon));

            var ship = Ship(gameServer, dbShip.ShipName, spawnPosition, spawnSector, components, weapons);
            var layer = GetShipLayer(ship);

            ship.TryAddComponent(new PlayerShip()
            {
                Username = player.User.Username,
            });

            ship.TryAddComponent(new WorldIcon()
            {
                Scale = new Vector2(0.75f),
                Texture = "Markers/ally_marker.png",
                Layer = layer,
            });

            ship.TryAddComponent(new WorldSpaceLabel()
            {
                TextSize = 20,
                BaseText = player.User.Username,
                Text = player.User.Username,
                Color = Veldrid.RgbaByte.White,
                TextOutline = 1,
                MarginBottom = 15,
            });

            ship.TryAddComponent(new Human());

            EntityUtility.SetNeedsTempNetworkSync<PlayerShip>(ship);
            EntityUtility.SetNeedsTempNetworkSync<WorldSpaceLabel>(ship);
            EntityUtility.SetNeedsTempNetworkSync<WorldIcon>(ship);

            return ship;

        } // PlayerShip

        public static Entity AlienShip(
            GameServer gameServer,
            ClassType shipClass,
            QualityType quality,
            Vector2 spawnPosition,
            Vector2I spawnSector,
            Vector2 sentryPosition,
            Vector2I sentrySector,
            int level)
        {
            var shipName = shipClass + " Alien";
            var shipData = GameDataManager.Ships[shipName];

            var components = new List<ShipPrefabComponent>();
            var weapons = new List<ShipPrefabWeapon>();

            for (int i = 0; i < shipData.Turrets.Count; i++)
            {
                weapons.Add(new ShipPrefabWeapon()
                {
                    Slot = i,
                    Seed = Guid.NewGuid().ToString(),
                    Quality = quality,
                });
            }

            var ship = Ship(gameServer, shipName, spawnPosition, spawnSector, components, weapons);
            var layer = GetShipLayer(ship);

            ship.TryAddComponent(new WorldSpaceLabel()
            {
                TextSize = 20,
                BaseText = shipName + " LVL " + level.ToString(),
                Text = shipName + " LVL " + level.ToString(),
                Color = Veldrid.RgbaByte.Red,
                TextOutline = 1,
                MarginBottom = 15,
            });

            ship.TryAddComponent(new WorldIcon()
            {
                Scale = new Vector2(0.75f),
                Texture = "Markers/enemy_marker.png",
                Layer = layer,
            });

            ship.TryAddComponent(new Alien());

            ship.TryAddComponent(new AISentry()
            {
                Sector = sentrySector,
                SentryPosition = sentryPosition,
                MaxEnemySearchRange = 50000,
                MaxEnemyChaseRange = 100000,
            });

            ship.TryAddComponent(new Loot()
            {
                Exp = shipData.ExpValue + level * 10,
                Bounty = shipData.BountyValue + level * 25,
            });

            EntityUtility.SetNeedsTempNetworkSync<WorldSpaceLabel>(ship);
            EntityUtility.SetNeedsTempNetworkSync<WorldIcon>(ship);

            return ship;

        } // AlienShip

    } // ShipPrefabs
}
