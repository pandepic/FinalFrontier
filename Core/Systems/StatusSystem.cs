using ElementEngine;
using ElementEngine.ECS;
using FinalFrontier.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier
{
    public static class StatusSystem
    {
        public static void RunShield(Group shieldGroup, GameTimer gameTimer)
        {
            foreach (var entity in shieldGroup.Entities)
            {
                ref var shield = ref entity.GetComponent<Shield>();

                if (shield.CurrentValue < shield.BaseValue)
                {
                    shield.CurrentValue += shield.RechargeRate * gameTimer.DeltaS;
                    if (shield.CurrentValue > shield.BaseValue)
                        shield.CurrentValue = shield.BaseValue;

                    EntityUtility.SetNeedsTempNetworkSync<Shield>(entity);
                }
            }
        }

    } // StatusSystem
}
