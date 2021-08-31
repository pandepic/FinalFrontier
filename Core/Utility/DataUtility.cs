using ElementEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Utility
{
    public static class DataUtility
    {
        public static List<DamageType> DamageTypes = Enum.GetValues<DamageType>().ToList();

        public static DamageType GetRandomDamageType()
        {
            return DamageTypes.GetRandomItem();
        }

    } // DataUtil
}
