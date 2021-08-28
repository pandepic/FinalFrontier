using ElementEngine;
using ElementEngine.ElementUI;
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
        }

        public override void Unload()
        {
            UIScreen?.HideDisable();
        }

        public override void Update(GameTimer gameTimer)
        {
            UIScreen?.Update(gameTimer);
        }

        public override void Draw(GameTimer gameTimer)
        {
            SpriteBatch.Begin(SamplerType.Linear);
            UIScreen?.Draw(SpriteBatch);
            SpriteBatch.End();
        }

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
