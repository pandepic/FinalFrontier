using ElementEngine;
using ElementEngine.ECS;
using FinalFrontier.Components;
using FinalFrontier.Database.Tables;
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
        public static Entity CreatePlayerShip(GameServer gameServer, Networking.Server.Player player, UserShip dbShip, Vector2 spawnPosition, Vector2I spawnSector)
        {
            var ship = gameServer.Registry.CreateEntity();
            var shipData = GameDataManager.Ships[dbShip.ShipName];
            var shipSprite = gameServer.SpriteAtlasData.GetSpriteRect(shipData.Sprite);

            var layer = 200000 + (ship.ID * 1000);

            ship.TryAddComponent(new Transform()
            {
                Rotation = 0f,
                Position = spawnPosition,
                SectorPosition = spawnSector,
            });

            ship.TryAddComponent(new Drawable()
            {
                AtlasRect = shipSprite,
                Texture = Globals.EntityAtlasTexture,
                Layer = layer,
                Scale = new Vector2(shipData.Scale),
                Color = Veldrid.RgbaByte.White,
            });

            ship.TryAddComponent(new WorldIcon()
            {
                Scale = new Vector2(0.5f),
                Texture = "Markers/ship_marker.png",
                Layer = layer,
            });

            var shipComponent = new Ship()
            {
                ShipType = shipData.Name,
                ShipComponentData = new Dictionary<ShipComponentType, ShipComponentSlotData>(),
                ShipWeaponData = new Dictionary<int, ShipWeaponSlotData>(),
            };

            foreach (var component in dbShip.Components)
            {
                shipComponent.ShipComponentData.Add(component.Slot, new ShipComponentSlotData()
                {
                    Slot = component.Slot,
                    Seed = component.Seed,
                    Quality = component.Quality,
                });
            }

            foreach (var weapon in dbShip.Weapons)
            {
                shipComponent.ShipWeaponData.Add(weapon.Slot, new ShipWeaponSlotData()
                {
                    Slot = weapon.Slot,
                    Seed = weapon.Seed,
                    Quality = weapon.Quality,
                });
            }

            ship.TryAddComponent(shipComponent);

            ship.TryAddComponent(new PlayerShip()
            {
                Username = player.User.Username,
            });
            
            EntityUtility.SetNeedsTempNetworkSync<Transform>(ship);
            EntityUtility.SetNeedsTempNetworkSync<Drawable>(ship);
            EntityUtility.SetNeedsTempNetworkSync<WorldIcon>(ship);
            EntityUtility.SetNeedsTempNetworkSync<Ship>(ship);
            EntityUtility.SetNeedsTempNetworkSync<PlayerShip>(ship);

            return ship;

        } // CreatePlayerShip

    } // ShipPrefabs
}
