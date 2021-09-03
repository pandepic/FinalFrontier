using ElementEngine;
using FinalFrontier.Components;
using SharpNeat.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.GameData
{
    public class ShipWeaponData
    {
        public TurretData TurretData;
        public ProjectileData ProjectileData;

        public ShipWeaponData(ShipWeaponSlotData slotData)
        {
            var rng = new FastRandom(MathHelper.GetSeedFromString(slotData.Seed));
            TurretData = GameDataManager.Turrets.GetRandomItem(rng);
            ProjectileData = GameDataManager.Projectiles.GetRandomItem(rng);
        }
    }
}
