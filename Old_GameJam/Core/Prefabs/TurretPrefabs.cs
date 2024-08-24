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
    public static class TurretPrefabs
    {
        public static Entity ShipTurret(GameServer gameServer, Entity parent, int layer, ShipWeaponSlotData slotData, ShipData shipData)
        {
            var turret = gameServer.Registry.CreateEntity();

            var turretClass = shipData.Turrets[slotData.Slot].Class;
            var shipWeaponData = new ShipWeaponData(slotData, turretClass);
            var turretSprite = gameServer.SpriteAtlasData.GetSpriteRect(shipWeaponData.TurretData.Sprite);

            turret.TryAddComponent(new Transform()
            {
                Parent = parent,
                Rotation = 0f,
                Position = shipData.Turrets[slotData.Slot].Position,
            });

            turret.TryAddComponent(new Drawable()
            {
                AtlasRect = turretSprite,
                Texture = Globals.EntityAtlasTexture,
                Layer = layer,
                Scale = new Vector2(shipWeaponData.TurretData.Scale),
                Color = Veldrid.RgbaByte.White,
            });

            turret.TryAddComponent(new Turret()
            {
                Parent = parent,
                Target = new Entity(),
                TargetRotation = 0f,
                WeaponData = shipWeaponData,
            });

            ref var ship = ref parent.GetComponent<Ship>();

            if (ship.Turrets.ContainsKey(slotData.Slot))
                ship.Turrets.Remove(slotData.Slot);

            ship.Turrets.Add(slotData.Slot, turret);

            EntityUtility.SetNeedsTempNetworkSync<Transform>(turret);
            EntityUtility.SetNeedsTempNetworkSync<Drawable>(turret);

            EntityUtility.SetSyncEveryTick<Transform>(turret);

            return turret;

        } // ShipTurret

        public static Entity TurretProjectile(GameServer gameServer, Entity parent, float rotation, ShipWeaponData weaponData, Entity target, Entity ship)
        {
            var projectile = gameServer.Registry.CreateEntity();
            var projectileSprite = gameServer.SpriteAtlasData.GetSpriteRect(weaponData.ProjectileData.Sprite);

            ref var parentTransform = ref parent.GetComponent<Transform>();
            ref var parentTurret = ref parent.GetComponent<Turret>();

            var spawnTransform = new Transform()
            {
                Parent = parent,
                Position = weaponData.TurretData.ProjectileSpawnPosition,
            };

            projectile.TryAddComponent(new Transform()
            {
                Rotation = rotation,
                Position = spawnTransform.TransformedPosition,
                SectorPosition = spawnTransform.TransformedSectorPosition,
            });

            var colour = weaponData.ProjectileData.Colour;

            projectile.TryAddComponent(new Drawable()
            {
                AtlasRect = projectileSprite,
                Texture = Globals.EntityAtlasTexture,
                Layer = Globals.LAYER_PROJECTILE,
                Scale = new Vector2(weaponData.ProjectileData.Scale),
                Color = new Veldrid.RgbaFloat(colour.X, colour.Y, colour.Z, colour.W).ToRgbaByte(),
            });

            projectile.TryAddComponent(new Projectile()
            {
                Parent = parent,
                DamageType = weaponData.DamageType,
                Damage = weaponData.Damage,
                Lifetime = weaponData.ProjectileLifetime,
            });

            var forwardVector = new Vector2(0f, -1f);
            var rotaterMatrix = Matrix3x2.CreateRotation(rotation.ToRadians());
            forwardVector = Vector2.TransformNormal(forwardVector, rotaterMatrix);

            projectile.TryAddComponent(new Physics()
            {
                Velocity = forwardVector * weaponData.MoveSpeed,
            });

            if (weaponData.ProjectileData.Type == ProjectileType.Missile)
            {
                projectile.TryAddComponent(new Missile()
                {
                    TurnSpeed = weaponData.TurnSpeed,
                    Target = target,
                    TargetRotation = rotation,
                    MoveSpeed = weaponData.MoveSpeed,
                });
            }

            if (ship.HasComponent<Human>())
                projectile.TryAddComponent(new Human());
            else if (ship.HasComponent<Alien>())
                projectile.TryAddComponent(new Alien());

            EntityUtility.SetNeedsTempNetworkSync<Transform>(projectile);
            EntityUtility.SetNeedsTempNetworkSync<Drawable>(projectile);

            EntityUtility.SetSyncEveryTick<Transform>(projectile);

            //Console.WriteLine($"Spawned projectile entity {projectile.ID}")

            return projectile;

        } // TurretProjectile

    } // TurretPrefabs
}
