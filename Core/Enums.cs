namespace FinalFrontier
{
    public enum NetworkPacketDataType
    {
        None,
        ServerStatus,
        Login,
        Register,
    }

    public enum GameStateType
    {
        Menu,
        Play,
    }

    public enum AudioType
    {
        Music,
        SFX,
        UI,
    }
    
    public enum RankType
    {
        Ensign,
        Lieutenant,
        LieutenantCommander,
        Commander,
        Captain,
        ViceAdmiral,
        Admiral,
    }

    public enum TeamType
    {
        Human,
        Alien,
        Pirate,
    }

    public enum ShipComponentType
    {
        Pilot,
        Gunner,
        Engineer,
        
        Engine,
        Shield,
        Armour,
        Powerplant,
    }
    
    public enum ProjectileType
    {
        Bullet,
        Missile,
    }
    
    public enum ComponentQualityType
    {
        Common,
        Uncommon,
        Rare,
        Legendary,
    }

    public enum ClassType
    {
        Small,
        Medium,
        Large,
    }
    
    public enum DamageType
    {
        Kinetic, // 50% to shields
        Explosive, // 100% to both
        Energy, // 50% to armour
    }
}
