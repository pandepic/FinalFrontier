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
    }
}
