global using Rectangle = ElementEngine.Rectangle;
using ElementEngine;

namespace FinalFrontier.Core;

public struct Resolution
{
    public int Width, Height;

    public Resolution(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public override string ToString()
    {
        return $"{Width}x{Height}";
    }
}

public static class Globals
{
    public static GameSettings GameSettings = new();

    #region Window Resolution
    public static List<Resolution> PossibleResolutions = new()
        {
            new Resolution(7680, 4320),
            new Resolution(3840, 2160),
            new Resolution(2560, 1440),
            new Resolution(1920, 1440),
            new Resolution(1920, 1200),
            new Resolution(1920, 1080),
            new Resolution(1680, 1050),
            new Resolution(1600, 1200),
            new Resolution(1600, 900),
            new Resolution(1440, 900),
            new Resolution(1366, 768),
            new Resolution(1360, 768),
            new Resolution(1280, 1440),
            new Resolution(1280, 1024),
            new Resolution(1280, 960),
            new Resolution(1280, 800),
            new Resolution(1280, 720),
        };
    #endregion
}
