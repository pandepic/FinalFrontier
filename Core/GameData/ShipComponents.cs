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

        public ShipEngineData(ShipComponentSlotData data)
        {
            var rng = new FastRandom(MathHelper.GetSeedFromString(data.Seed));

            switch (data.Quality)
            {
                case ComponentQualityType.Common:
                    {
                        var speedBonus = rng.Next(5, 20);
                        var turnBonus = rng.Next(5, 20);

                        MoveSpeedBonus = 1f + (speedBonus / 100f);
                        TurnSpeedBonus = 1f + (turnBonus / 100f);
                    }
                    break;

                case ComponentQualityType.Uncommon:
                    {
                        var speedBonus = rng.Next(20, 40);
                        var turnBonus = rng.Next(20, 40);

                        MoveSpeedBonus = 1f + (speedBonus / 100f);
                        TurnSpeedBonus = 1f + (turnBonus / 100f);
                    }
                    break;

                case ComponentQualityType.Rare:
                    {
                        var speedBonus = rng.Next(40, 60);
                        var turnBonus = rng.Next(40, 60);

                        MoveSpeedBonus = 1f + (speedBonus / 100f);
                        TurnSpeedBonus = 1f + (turnBonus / 100f);
                    }
                    break;

                case ComponentQualityType.Legendary:
                    {
                        var speedBonus = rng.Next(60, 100);
                        var turnBonus = rng.Next(60, 100);

                        MoveSpeedBonus = 1f + (speedBonus / 100f);
                        TurnSpeedBonus = 1f + (turnBonus / 100f);
                    }
                    break;
            }
        }

    } // ShipEngineData
}
