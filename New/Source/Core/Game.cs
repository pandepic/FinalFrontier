using ElementEngine;
using Newtonsoft.Json;
using Veldrid;

namespace FinalFrontier.Core;

public class Game : BaseGame
{
    private GameSettings _settings => Globals.GameSettings;

    public override void Load()
    {
        File.Delete("log.txt");
        Logging.Load(Logging.CreateBasicConfig("log.txt"));

        var displayMode = GetCurrentDisplayMode();

        // remove resolutions bigger than the current screen
        for (var i = Globals.PossibleResolutions.Count - 1; i >= 0; i--)
        {
            var resolution = Globals.PossibleResolutions[i];

            if (resolution.Width > displayMode.w || resolution.Height > displayMode.h)
                Globals.PossibleResolutions.RemoveAt(i);
        }

        LoadSettings();

        var windowPosition = new Vector2I(0, y: 0);
        var assetsPath = "Mods";
        var graphicsDeviceDebugMode = false;

#if DEBUG
        //var displays = Veldrid.Sdl2.Sdl2Native.SDL_GetNumVideoDisplays();

        //if (displays == 3)
        //    windowPosition.X += 2600;

        graphicsDeviceDebugMode = true;
#endif

        var windowRect = new Rectangle();
        var windowState = _settings.BorderlessFullscreen ? WindowState.BorderlessFullScreen : WindowState.Normal;

        if (_settings.BorderlessFullscreen)
        {
            windowRect.Width = displayMode.w;
            windowRect.Height = displayMode.h;
        }
        else
        {
            var screenSize = new Vector2I(displayMode.w, displayMode.h);
            var screenCenter = (screenSize / 2) - (_settings.Resolution / 2);

            windowRect.Width = _settings.Resolution.X;
            windowRect.Height = _settings.Resolution.Y;
            windowRect.X = screenCenter.X;
            windowRect.Y = screenCenter.Y;
        }

        SetupWindow(
            windowRect,
            "Final Frontier",
            graphicsBackend: GraphicsBackend.Direct3D11,
            vsync: _settings.Vsync,
            windowState: windowState,
            debug: graphicsDeviceDebugMode);

        SetupAssets(assetsPath);
    }

    private void LoadSettings()
    {
        if (!File.Exists("Settings.json"))
        {
            SaveSettings();
            return;
        }

        Globals.GameSettings = System.Text.Json.JsonSerializer.Deserialize<GameSettings>(
            File.ReadAllText("Settings.json"),
            new System.Text.Json.JsonSerializerOptions()
        {
            IncludeFields = true,
        })!;
    }

    private void SaveSettings()
    {
        var json = System.Text.Json.JsonSerializer.Serialize(
            _settings,
            new System.Text.Json.JsonSerializerOptions()
        {
            IncludeFields = true,
        });

        File.WriteAllText("Settings.json", json);
    }

    public override void Draw(GameTimer gameTimer)
    {
        
    }
}
