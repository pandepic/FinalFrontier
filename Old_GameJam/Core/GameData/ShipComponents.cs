using ElementEngine;
using FinalFrontier.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.GameData
{
    public class ShipComponentData
    {
        public ShipComponentSlotData Data;

        public ShipComponentData(ShipComponentSlotData data)
        {
            Data = data;
        }

        public float ToPercentage(float value)
        {
            return (value - 1f) * 100f;
        }
    }

    public class ShipEngineData : ShipComponentData
    {
        public float MoveSpeedBonus;
        public float TurnSpeedBonus;
        public float WarpSpeedBonus;
        public float WarpCooldownReduction;

        public ShipEngineData(ShipComponentSlotData data) : base(data)
        {
            var rng = new Random(MathHelper.GetSeedFromString(data.Seed));

            switch (data.Quality)
            {
                case QualityType.Common:
                    {
                        var speedBonus = rng.Next(5, 20);
                        var turnBonus = rng.Next(5, 20);
                        var warpSpeedBonus = rng.Next(5, 20);
                        var warpCooldownBonus = rng.Next(0, 5);

                        MoveSpeedBonus = 1f + (speedBonus / 100f);
                        TurnSpeedBonus = 1f + (turnBonus / 100f);
                        WarpSpeedBonus = 1f + (warpSpeedBonus / 100f);
                        WarpCooldownReduction = 0.1f * warpCooldownBonus;
                    }
                    break;

                case QualityType.Uncommon:
                    {
                        var speedBonus = rng.Next(20, 40);
                        var turnBonus = rng.Next(20, 40);
                        var warpSpeedBonus = rng.Next(20, 40);
                        var warpCooldownBonus = rng.Next(5, 10);

                        MoveSpeedBonus = 1f + (speedBonus / 100f);
                        TurnSpeedBonus = 1f + (turnBonus / 100f);
                        WarpSpeedBonus = 1f + (warpSpeedBonus / 100f);
                        WarpCooldownReduction = 0.1f * warpCooldownBonus;
                    }
                    break;

                case QualityType.Rare:
                    {
                        var speedBonus = rng.Next(40, 60);
                        var turnBonus = rng.Next(40, 60);
                        var warpSpeedBonus = rng.Next(40, 60);
                        var warpCooldownBonus = rng.Next(10, 15);

                        MoveSpeedBonus = 1f + (speedBonus / 100f);
                        TurnSpeedBonus = 1f + (turnBonus / 100f);
                        WarpSpeedBonus = 1f + (warpSpeedBonus / 100f);
                        WarpCooldownReduction = 0.1f * warpCooldownBonus;
                    }
                    break;

                case QualityType.Legendary:
                    {
                        var speedBonus = rng.Next(60, 100);
                        var turnBonus = rng.Next(60, 100);
                        var warpSpeedBonus = rng.Next(60, 100);
                        var warpCooldownBonus = rng.Next(15, 20);

                        MoveSpeedBonus = 1f + (speedBonus / 100f);
                        TurnSpeedBonus = 1f + (turnBonus / 100f);
                        WarpSpeedBonus = 1f + (warpSpeedBonus / 100f);
                        WarpCooldownReduction = 0.1f * warpCooldownBonus;
                    }
                    break;
            }
        }

        public override string ToString()
        {
            return $"{Data.Quality} Engine: [+{ToPercentage(MoveSpeedBonus):0}% Move Speed] [+{ToPercentage(TurnSpeedBonus):0}% Turn Speed]"
                + $" [+{ToPercentage(WarpSpeedBonus):0}% Warp Speed] [-{WarpCooldownReduction:0.00} Warp Charge Time]";
        }

    } // ShipEngineData

    public class ShipShieldData : ShipComponentData
    {
        public float ShieldBonus;
        public float RechargeBonus;

        public ShipShieldData(ShipComponentSlotData data) : base(data)
        {
            var rng = new Random(MathHelper.GetSeedFromString(data.Seed));

            switch (data.Quality)
            {
                case QualityType.Common:
                    {
                        ShieldBonus = 1f + (rng.Next(5, 20) / 100f);
                        RechargeBonus = rng.Next(1, 3);
                    }
                    break;

                case QualityType.Uncommon:
                    {
                        ShieldBonus = 1f + (rng.Next(20, 40) / 100f);
                        RechargeBonus = rng.Next(3, 6);
                    }
                    break;

                case QualityType.Rare:
                    {
                        ShieldBonus = 1f + (rng.Next(40, 60) / 100f);
                        RechargeBonus = rng.Next(6, 9);
                    }
                    break;

                case QualityType.Legendary:
                    {
                        ShieldBonus = 1f + (rng.Next(60, 100) / 100f);
                        RechargeBonus = rng.Next(9, 25);
                    }
                    break;
            }
        }

        public override string ToString()
        {
            return $"{Data.Quality} Shield: [+{ToPercentage(ShieldBonus):0.00}% Shield HP] [+{RechargeBonus:0.00} Shield/Sec]";
        }

    } // ShipShieldData

    public class ShipArmourData : ShipComponentData
    {
        public float ArmourBonus;

        public ShipArmourData(ShipComponentSlotData data) : base(data)
        {
            var rng = new Random(MathHelper.GetSeedFromString(data.Seed));

            switch (data.Quality)
            {
                case QualityType.Common:
                    {
                        ArmourBonus = 1f + (rng.Next(5, 20) / 100f);
                    }
                    break;

                case QualityType.Uncommon:
                    {
                        ArmourBonus = 1f + (rng.Next(20, 40) / 100f);
                    }
                    break;

                case QualityType.Rare:
                    {
                        ArmourBonus = 1f + (rng.Next(40, 60) / 100f);
                    }
                    break;

                case QualityType.Legendary:
                    {
                        ArmourBonus = 1f + (rng.Next(60, 100) / 100f);
                    }
                    break;
            }
        }

        public override string ToString()
        {
            return $"{Data.Quality} Armour: [+{ToPercentage(ArmourBonus):0.00}% Armour HP]";
        }

    } // ShipArmourData
}
