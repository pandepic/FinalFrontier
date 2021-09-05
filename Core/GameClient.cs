using ElementEngine;
using ElementEngine.ECS;
using FinalFrontier.Networking;
using System;
using System.Collections.Generic;

namespace FinalFrontier
{
    public class GameClient : BaseGame
    {
        public static NetworkClient NetworkClient;

        public Dictionary<GameStateType, GameState> GameStates { get; set; } = new Dictionary<GameStateType, GameState>();

        public string WorldSeed;
        public double WorldTime;
        public GalaxyGenerator GalaxyGenerator;

        // ECS
        public Registry Registry;
        public Group DrawableGroup;

        public override void Load()
        {
            var displayMode = GetCurrentDisplayMode();
            SettingsManager.LoadFromPath("Settings.xml");

            var strWidth = SettingsManager.GetSetting<string>("Window", "Width");
            var strHeight = SettingsManager.GetSetting<string>("Window", "Height");

            // default to fullscreen borderless
            if (string.IsNullOrWhiteSpace(strWidth) || string.IsNullOrWhiteSpace(strHeight))
            {
                SettingsManager.UpdateSetting("Window", "Width", displayMode.w);
                SettingsManager.UpdateSetting("Window", "Height", displayMode.h);
                SettingsManager.UpdateSetting("Window", "BorderlessFullscreen", true);
            }

            var vsync = SettingsManager.GetSetting<bool>("Window", "Vsync");
            var borderless = SettingsManager.GetSetting<bool>("Window", "BorderlessFullscreen");

            var windowRect = new ElementEngine.Rectangle()
            {
                X = 100,
                Y = 100,
                Width = SettingsManager.GetSetting<int>("Window", "Width"),
                Height = SettingsManager.GetSetting<int>("Window", "Height")
            };

            var windowState = borderless ? Veldrid.WindowState.BorderlessFullScreen : Veldrid.WindowState.Normal;

            if (borderless)
            {
                windowRect.Width = displayMode.w;
                windowRect.Height = displayMode.h;
            }

            // remove resolutions bigger than the current screen
            for (var i = Globals.PossibleResolutions.Count - 1; i >= 0; i--)
            {
                var resolution = Globals.PossibleResolutions[i];

                if (resolution.Width > displayMode.w || resolution.Height > displayMode.h)
                    Globals.PossibleResolutions.RemoveAt(i);
            }

            SetupWindow(windowRect, "Final Frontier", vsync: vsync, windowState: windowState);
            Window.Resizable = false;

            AssetManager.Load("Content", LoadAssetsMode.AutoPrependDir | LoadAssetsMode.AutoFind);
            InputManager.LoadGameControls();

            Globals.Load();
            Globals.SetLanguage(SettingsManager.GetSetting<string>("UI", "Language"));
            GameDataManager.Load();

            GameStates.Add(GameStateType.Menu, new GameStateMenu(this));
            GameStates.Add(GameStateType.Play, new GameStatePlay(this));
            GameStates.Add(GameStateType.Loading, new GameStateLoading(this));

            UICursors.Setup();
            UICursors.SetCursor(UICursorType.Normal);

            UpdateAudioVolume();

            NetworkClient = new NetworkClient(this);
            SetGameState(GameStateType.Menu);

            SoundManager.Play(AssetManager.LoadAudioSourceByExtension("Audio/Bold - Full.ogg"), (int)AudioType.Music, true);

        } // Load

        public override void Update(GameTimer gameTimer)
        {
            NetworkClient.Update(gameTimer);
        }

        public override void Draw(GameTimer gameTimer)
        {
        }

        public override void Exit()
        {
            SaveSettings();
        }

        public static void SaveSettings() => SettingsManager.Save("Settings.xml");

        public static void UpdateAudioVolume()
        {
            SoundManager.SetMasterVolume(SettingsManager.GetSetting<float>("Sound", "MasterVolume"));
            SoundManager.SetVolume((int)AudioType.Music, SettingsManager.GetSetting<float>("Sound", "MusicVolume"));
            SoundManager.SetVolume((int)AudioType.SFX, SettingsManager.GetSetting<float>("Sound", "SFXVolume"));
            SoundManager.SetVolume((int)AudioType.UI, SettingsManager.GetSetting<float>("Sound", "UIVolume"));
        }

        public void SetGameState(GameStateType type)
        {
            SetGameState(GameStates[type]);
        }

        #region Network Events
        public void LoginSuccess(string worldSeed)
        {
            WorldSeed = worldSeed;
            SetGameState(GameStateType.Loading);
        }

        public void OnServerDisconnected()
        {
            if (CurrentGameState is INetworkGameState networkState)
                networkState.OnServerDisconnected();
        }
        #endregion

    } // GameClient
}
