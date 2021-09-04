using ElementEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.GameData
{
    public class ShipTurretData
    {
        public ClassType Class;
        public Vector2 Position;
    }

    public class ShipData
    {
        public RankType RequiredRank;

        public string Name;
        public int Cost;
        public ClassType Class;
        public string Atlas;
        public string Sprite;
        public float Scale;
        public bool HasHangar;
        public float BaseMoveSpeed;
        public float BaseTurnSpeed;
        public float BaseShield;
        public float BaseArmour;
        public float BaseShieldRegen; // per second
        public List<ShipTurretData> Turrets = new List<ShipTurretData>();
        public int ExpValue;
        public int BountyValue;
    }
}
