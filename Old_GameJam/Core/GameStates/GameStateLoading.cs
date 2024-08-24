using ElementEngine;
using ElementEngine.ECS;
using ElementEngine.ElementUI;
using FinalFrontier.Components;
using FinalFrontier.Networking;
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
        public GameClient GameClient;
        public UIScreen UIScreen;

        public int IdleFrames = 0;
        
        public GameStateLoading(GameClient client)
        {
            GameClient = client;
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

            ClientGlobals.PlayerShip = new Entity();
            GameClient.Registry = new Registry();

            NetworkSyncManager.Registry = GameClient.Registry;
            NetworkSyncManager.LoadShared();
            NetworkSyncManager.LoadClient();

            GameClient.DrawableGroup = GameClient.Registry.RegisterGroup<Transform, Drawable>();
            GameClient.DrawableGroup.ExcludeTypes = new Type[] { typeof(OrbitalBody) };

            GameClient.GalaxyGenerator = new GalaxyGenerator(GameClient.Registry, GameClient.WorldSeed, false);

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
                GameClient.GalaxyGenerator.GenerateGalaxy();
                stopWatch.Stop();
                Logging.Information("Generated galaxy with {stars} stars in {time:0.00} ms.", GameClient.GalaxyGenerator.GalaxyStars.Count, stopWatch.ElapsedMilliseconds);
                GameClient.SetGameState(GameStateType.Play);
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
