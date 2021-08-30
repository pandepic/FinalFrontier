using ElementEngine;
using ElementEngine.ECS;
using FinalFrontier.Networking;
using System;
using System.Diagnostics;

namespace FinalFrontier
{
    public class GameServer : BaseGame
    {
        public static readonly string WorldSeed = Guid.NewGuid().ToString();

        public GalaxyGenerator GalaxyGenerator;
        public NetworkServer NetworkServer;

        public Registry Registry;

        public override void Load()
        {
            var windowRect = new ElementEngine.Rectangle()
            {
                X = 100,
                Y = 100,
                Width = 1600,
                Height = 900
            };

            SetupWindow(windowRect, "SERVER: Final Frontier", vsync: true, windowState: Veldrid.WindowState.Normal);
            Window.Resizable = false;

            AssetManager.Load("Content", LoadAssetsMode.AutoPrependDir | LoadAssetsMode.AutoFind);
            Globals.Load();
            GameDataManager.Load();

            Registry = new Registry();

            Logging.Information("Generating galaxy...");
            var stopWatch = Stopwatch.StartNew();
            GalaxyGenerator = new GalaxyGenerator(Registry, WorldSeed, true);
            GalaxyGenerator.GenerateGalaxy();
            stopWatch.Stop();
            Logging.Information("Generated galaxy with {stars} stars in {time:0.00} ms.", GalaxyGenerator.GalaxyStars.Count);

            NetworkServer = new NetworkServer();
            NetworkServer.Load();
        }

        public override void Update(GameTimer gameTimer)
        {
            NetworkServer.Update(gameTimer);
        }

        public override void Draw(GameTimer gameTimer)
        {
        }

        public override void Exit()
        {
        }

    } // GameServer
}
