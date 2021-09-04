using ElementEngine;
using ElementEngine.TexturePacker;
using SharpNeat.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier
{
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
        #region Constants
        public const int LAYER_PROJECTILE = 100000000;

        public const float BASE_SECTOR_WARP_SPEED = 25000f;
        public const float BASE_GALAXY_WARP_SPEED = 2500000f;
        public const float BASE_WARP_COOLDOWN = 5f; // in seconds
        public const float WARP_DRIVE_SECTOR_DISTANCE = 25000f;
        public const float WARP_DRIVE_GALAXY_DISTANCE = 300000f;
        public const float WARP_DRIVE_STOP_DISTANCE = 2500f;

        public const int MAX_LABEL_ZOOM_LEVEL = 10;

        public static readonly float GalaxySectorScale = 1000000;
        public static readonly int GalaxyGridSize = 200;

        public static float ALIEN_WAVE_SPAWN_TIMER = 5f * 60f; // 5 minutes
        #endregion

        public static FastRandom RNG = new FastRandom();

        public static List<DamageType> DamageTypes = Enum.GetValues<DamageType>().ToList();

        public static List<Vector2I> SurroundingPositions = new List<Vector2I>()
        {
            new Vector2I(-1, -1), // top left
            new Vector2I(0, -1), // top
            new Vector2I(1, -1), // top right

            new Vector2I(-1, 0), // left
            new Vector2I(1, 0), // right

            new Vector2I(-1, 1), // bottom left
            new Vector2I(0, 1), // bottom
            new Vector2I(1, 1), // bottom right
        };

        public static List<Resolution> PossibleResolutions = new List<Resolution>()
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
        };

        public static void SaveResolution(Resolution resolution)
        {
            SettingsManager.UpdateSetting("Window", "Width", resolution.Width.ToString());
            SettingsManager.UpdateSetting("Window", "Height", resolution.Height.ToString());
        }

        public static string CurrentLanguage = "";
        public static List<string> Languages = new List<string>()
        {
            "English",
        };

        public static void SetLanguage(string language)
        {
            LocalisationManager.SetLanguage($"Languages/{language}.json");
            SettingsManager.UpdateSetting("UI", "Language", language);
            CurrentLanguage = language;
        }

        public static TexturePackerAtlas UIAtlas;
        public static TexturePackerAtlas EntityAtlas;
        public static string EntityAtlasData = "Textures/entity_atlas.json";
        public static string EntityAtlasTexture = "Textures/entity_atlas.png";

        public static string ServerAddress = "66.70.191.87";
        public static int ServerPort = 21461;
        public const string ConnectionKey = "b0beb091-902e-418d-b60c-2c7f4156696a";

        public static void Load()
        {
            UIAtlas = AssetManager.LoadTexturePackerAtlas("Textures/ui_atlas.png", "Textures/ui_atlas.json");
            EntityAtlas = AssetManager.LoadTexturePackerAtlas(EntityAtlasTexture, EntityAtlasData);

#if DEBUG
            ServerAddress = "localhost";
#endif
        }

        public static List<string> ColonyNames = new List<string>()
        {
            "Rome",
            "Ariminum",
            "Belum",
            "Tarraco",
            "Modoetia",
            "Salernum",
            "Aquileia",
            "Ascrivium",
            "Palma",
            "Massa",
            "Pompaelo",
            "Naissus",
            "Brigantium",
            "Florentia",
            "Vesontio",
            "Lugdunum",
            "Noviodunum",
            "Siscia",
            "Marsonia",
            "Caesarea",
            "Novaesium",
            "Ovilava",
            "Turicum",
            "Argentoratum",
        };

    } // Globals
}
