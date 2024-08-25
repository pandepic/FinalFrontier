using ElementEngine;

namespace FinalFrontier.Core;

public class GameplaySettings
{
    public bool ZoomToCursor = true;
}

public class GameSettings
{
    public Vector2I Resolution = new(1920, 1080);
    public bool BorderlessFullscreen = false;
    public bool Vsync = true;

    public GameplaySettings Gameplay = new();
}
