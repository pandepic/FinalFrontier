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
            
            var shipWeaponData = new ShipWeaponData(slotData);
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

            EntityUtility.SetNeedsTempNetworkSync<Transform>(turret);
            EntityUtility.SetNeedsTempNetworkSync<Drawable>(turret);
            EntityUtility.SetSyncEveryTick<Transform>(turret);

            return turret;
        } // ShipTurret
    }
}
