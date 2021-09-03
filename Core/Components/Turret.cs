using ElementEngine.ECS;
using FinalFrontier.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Components
{
    public struct Turret
    {
        public Entity Parent;
        public ShipWeaponData WeaponData;
    }
}
