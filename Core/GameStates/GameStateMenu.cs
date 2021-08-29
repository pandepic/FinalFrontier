using ElementEngine;
using ElementEngine.ElementUI;
using ElementEngine.Timer;
using FinalFrontier.Networking;
using FinalFrontier.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier
{
    public class GameStateMenu : GameState
    {
        public GameClient Client;
        public UIScreen UIScreen;
        public SpriteBatch2D SpriteBatch;

        public CallbackTimer ServerStatusTimer;

        private string _prevServerStatus = "";

        public GameStateMenu(GameClient client)
        {
            Client = client;
        }

        public override void Initialize()
        {
            SpriteBatch = new SpriteBatch2D();
            UIScreen = UIBuilderMenu.Build(this);
        }

        public override void Load()
        {
            UIScreen?.ShowEnable();

            ServerStatusTimer = TimerManager.AddTimer(new CallbackTimer(5, true, ServerStatusTimer_Tick));
            ServerStatusTimer.Start();
        }

        public override void Unload()
        {
            UIScreen?.HideDisable();
            ServerStatusTimer.Stop();
        }

        public override void Update(GameTimer gameTimer)
        {
            var serverStatusLabel = UIScreen.FindChildByName<UILabel>("ServerStatus", true);

            string serverStatus;

            if (GameClient.NetworkClient.IsConnected)
                serverStatus = $"Online - {GameClient.NetworkClient.ServerPlayers} Players";
            else
                serverStatus = "Offline";

            if (_prevServerStatus != serverStatus)
            {
                serverStatusLabel.Text = LocalisationManager.GetString("ServerStatus", ("STATUS", serverStatus));
                _prevServerStatus = serverStatus;
            }

            UIScreen?.Update(gameTimer);
        }

        public override void Draw(GameTimer gameTimer)
        {
            SpriteBatch.Begin(SamplerType.Linear);
            UIScreen?.Draw(SpriteBatch);
            SpriteBatch.End();
        }

        #region Timer Callbacks
        public void ServerStatusTimer_Tick()
        {
            if (!GameClient.NetworkClient.IsConnected)
                return;

            ClientPacketSender.ServerStatus();
        }
        #endregion

        #region UI Callbacks
        public void Settings_OnClick(UIOnClickArgs args)
        {
            UIBuilderMenuSettings.ResetSettings();

            var settingsContainer = UIScreen.FindChildByName<UIContainer>("SettingsContainer", true);
            settingsContainer.ShowEnable();
        }

        public void Exit_OnClick(UIOnClickArgs args)
        {
            Client.Quit();
        }
        #endregion

    } // GameStateMenu
}
