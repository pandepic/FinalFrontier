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
        } // RunShield

        public static void RunArmour(GameServer gameServer, Group armourGroup, GameTimer gameTimer)
        {
            foreach (var entity in armourGroup.Entities)
            {
                ref var armour = ref entity.GetComponent<Armour>();
                ref var transform = ref entity.GetComponent<Transform>();

                if (gameServer.ServerWorldManager.ColonisedSectors.Contains(transform.TransformedSectorPosition))
                {
                    var healRate = armour.BaseValue * 0.05f;

                    armour.CurrentValue += healRate * gameTimer.DeltaS;
                    if (armour.CurrentValue > armour.BaseValue)
                        armour.CurrentValue = armour.BaseValue;

                    EntityUtility.SetNeedsTempNetworkSync<Armour>(entity);
                }
            }
        } // RunArmour

    } // StatusSystem
}
