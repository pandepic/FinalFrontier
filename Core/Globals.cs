﻿using ElementEngine;
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
        public static FastRandom RNG = new FastRandom();

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

        public static string ServerAddress = "66.70.191.87";
        public static int ServerPort = 21461;
        public const string ConnectionKey = "b0beb091-902e-418d-b60c-2c7f4156696a";

        public static void Load()
        {
            UIAtlas = AssetManager.LoadTexturePackerAtlas("Textures/ui_atlas.png", "Textures/ui_atlas.json");
            EntityAtlas = AssetManager.LoadTexturePackerAtlas("Textures/entity_atlas.png", "Textures/entity_atlas.json");

#if DEBUG
            ServerAddress = "localhost";
#endif
        }

    } // Globals
}
