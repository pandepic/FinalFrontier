﻿namespace FinalFrontier
{
    public enum NetworkPacketDataType
    {
        None,
        ServerStatus,
        Login,
        Register,
        JoinGame,
        WorldUpdate,
        PlayerMoveToPosition,
        DestroyEntity,
        Logout,
        ChatMessage,
        EquipComponent,
        EquipWeapon,
        SellItem,
        BuyShip,

        // Sync components
        SyncTransform,
        SyncDrawable,
        SyncColony,
        SyncWorldIcon,
        SyncShip,
        SyncPlayerShip,
        SyncWorldSpaceLabel,
        SyncShield,
        SyncArmour,
        SyncInventory,
    }

    public enum GameStateType
    {
        Menu,
        Play,
        Loading,
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

    public enum ShipComponentType
    {
        Engine,
        Shield,
        Armour,
    }
    
    public enum ProjectileType
    {
        Bullet,
        Missile,
    }
    
    public enum QualityType
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
