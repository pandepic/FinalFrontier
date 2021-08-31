using ElementEngine;
using ElementEngine.ECS;
using ElementEngine.ElementUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier
{
    public class GameStateLoading : GameState
    {
        public SpriteBatch2D SpriteBatch;
        public GameClient Client;
        public UIScreen UIScreen;

        public int IdleFrames = 0;
        
        public GameStateLoading(GameClient client)
        {
            Client = client;
        }
        
        public override void Initialize()
        {
            SpriteBatch = new SpriteBatch2D();

            UIScreen = new UIScreen();

            var background = new UIImage("Background", new UIImageStyle(new UISpriteStatic("Backgrounds/menu_bg.png"))
            {
                IgnoreParentPadding = true,
                UISize = new UISize() { ParentWidth = true, ParentHeight = true, FillType = UISizeFillType.Cover },
            });
            UIScreen.AddChild(background);

            var title = new UILabel("Title", UITheme.TitleLabelStyle, LocalisationManager.GetString("Loading"));
            UIScreen.AddChild(title);
        }

        public override void Load()
        {
            IdleFrames = 0;

            Client.Registry = new Registry();
            Client.GalaxyGenerator = new GalaxyGenerator(Client.Registry, Client.WorldSeed, false);

            UIScreen?.ShowEnable();
        }

        public override void Unload()
        {
            UIScreen?.HideDisable();
        }

        public override void Update(GameTimer gameTimer)
        {
            UIScreen.Update(gameTimer);

            IdleFrames += 1;

            if (IdleFrames == 5)
            {
                Logging.Information("Generating galaxy...");
                var stopWatch = Stopwatch.StartNew();
                Client.GalaxyGenerator.GenerateGalaxy();
                stopWatch.Stop();
                Logging.Information("Generated galaxy with {stars} stars in {time:0.00} ms.", Client.GalaxyGenerator.GalaxyStars.Count, stopWatch.ElapsedMilliseconds);
                Client.SetGameState(GameStateType.Play);
            }
        }

        public override void Draw(GameTimer gameTimer)
        {
            SpriteBatch.Begin(SamplerType.Linear);
            UIScreen.Draw(SpriteBatch);
            SpriteBatch.End();
        }

    } // GameStateLoading
}
