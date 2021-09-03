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
    public class ShipComponentData { }

    public class ShipEngineData : ShipComponentData
    {
        public float MoveSpeedBonus;
        public float TurnSpeedBonus;
        public float WarpSpeedBonus;
        public float WarpCooldownReduction;

        public ShipEngineData(ShipComponentSlotData data)
        {
            var rng = new FastRandom(MathHelper.GetSeedFromString(data.Seed));

            switch (data.Quality)
            {
                case ComponentQualityType.Common:
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

                case ComponentQualityType.Uncommon:
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

                case ComponentQualityType.Rare:
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

                case ComponentQualityType.Legendary:
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

    } // ShipEngineData
}
