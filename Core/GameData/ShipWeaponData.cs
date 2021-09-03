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

        public float Cooldown;
        public DamageType DamageType;
        public float Damage;
        public float Range;
        public float TurretTurnRate;
        public float MoveSpeed;
        public float TurnSpeed;
        public float ProjectileLifetime;

        public ShipWeaponData(ShipWeaponSlotData slotData)
        {
            var rng = new FastRandom(MathHelper.GetSeedFromString(slotData.Seed));

            TurretData = GameDataManager.Turrets.GetRandomItem(rng);
            ProjectileData = GameDataManager.Projectiles.GetRandomItem(rng);

            if (ProjectileData.Type == ProjectileType.Missile)
                DamageType = DamageType.Explosive;
            else
                DamageType = Globals.DamageTypes.GetRandomItem(rng);

            switch (slotData.Quality)
            {
                case ComponentQualityType.Common:
                    {
                    }
                    break;

                case ComponentQualityType.Uncommon:
                    {
                    }
                    break;

                case ComponentQualityType.Rare:
                    {
                    }
                    break;

                case ComponentQualityType.Legendary:
                    {
                    }
                    break;
            }

        } // constructor

    } // ShipWeaponData
}
