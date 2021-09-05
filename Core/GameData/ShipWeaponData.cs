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
        public static Dictionary<QualityType, (int Min, int Max)> DamageRanges = new Dictionary<QualityType, (int Min, int Max)>()
        {
            { QualityType.Common, (60, 150) },
            { QualityType.Uncommon, (150, 225) },
            { QualityType.Rare, (225, 300) },
            { QualityType.Legendary, (300, 450) },
        };

        public static Dictionary<QualityType, (int Min, int Max)> DPSRanges = new Dictionary<QualityType, (int Min, int Max)>()
        {
            { QualityType.Common, (150, 300) },
            { QualityType.Uncommon, (225, 450) },
            { QualityType.Rare, (300, 600) },
            { QualityType.Legendary, (450, 900) },
        };

        public static Dictionary<ClassType, float> DamageMultipliers = new Dictionary<ClassType, float>()
        {
            { ClassType.Small, 0.25f },
            { ClassType.Medium, 0.5f },
            { ClassType.Large, 1f },
        };

        public static Dictionary<ClassType, (int Min, int Max)> TurretTurnRateRanges = new Dictionary<ClassType, (int Min, int Max)>()
        {
            { ClassType.Small, (600, 1000) },
            { ClassType.Medium, (400, 600) },
            { ClassType.Large, (200, 400) },
        };

        public static Dictionary<ClassType, (int Min, int Max)> MissileMoveSpeedRanges = new Dictionary<ClassType, (int Min, int Max)>()
        {
            { ClassType.Small, (1000, 2500) },
            { ClassType.Medium, (800, 1900) },
            { ClassType.Large, (600, 1300) },
        };

        public static Dictionary<ClassType, (int Min, int Max)> MissileTurnSpeedRanges = new Dictionary<ClassType, (int Min, int Max)>()
        {
            { ClassType.Small, (500, 800) },
            { ClassType.Medium, (300, 500) },
            { ClassType.Large, (150, 300) },
        };

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
        public float MaxFiringAngle = 5f;

        public float DPS
        {
            get => (1f / Cooldown) * Damage;
            set
            {
                Cooldown = value / Damage;
            }
        }

        public ShipWeaponData(ShipWeaponSlotData slotData, ClassType classType)
        {
            var rng = new FastRandom(MathHelper.GetSeedFromString(slotData.Seed));

            TurretData = GameDataManager.Turrets.Where(t => t.Value.Class == classType).ToList().GetRandomItem(rng).Value;
            ProjectileData = GameDataManager.Projectiles.GetRandomItem(rng);

            Range = rng.Next(3000, 10000);
            MoveSpeed = rng.Next(2500, 5000);
            TurnSpeed = 0;

            var turnRateRange = TurretTurnRateRanges[classType];
            TurretTurnRate = rng.Next(turnRateRange.Min, turnRateRange.Max);

            var damageRange = DamageRanges[slotData.Quality];
            Damage = rng.Next(damageRange.Min, damageRange.Max) * DamageMultipliers[classType];

            var dpsRange = DPSRanges[slotData.Quality];
            DPS = rng.Next(dpsRange.Min, dpsRange.Max) * DamageMultipliers[classType];

            if (ProjectileData.Type == ProjectileType.Missile)
            {
                var moveSpeedRange = MissileMoveSpeedRanges[classType];
                MoveSpeed = rng.Next(moveSpeedRange.Min, moveSpeedRange.Max);

                var turnSpeedRange = MissileTurnSpeedRanges[classType];
                TurnSpeed = rng.Next(turnSpeedRange.Min, turnSpeedRange.Max);

                DamageType = DamageType.Explosive;
                Range = rng.Next(8000, 15000);
            }
            else
            {
                DamageType = Globals.DamageTypes.GetRandomItem(rng);
            }

            ProjectileLifetime = (Range / MoveSpeed) * 2;

        } // constructor

    } // ShipWeaponData
}
