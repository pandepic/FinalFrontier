using ElementEngine;
using ElementEngine.ECS;
using ElementEngine.TexturePacker;
using ElementEngine.UI;
using FinalFrontier.Components;
using FinalFrontier.Database.Tables;
using FinalFrontier.Networking;
using ImGuiNET;
using System;
using System.Diagnostics;
using System.Numerics;

namespace FinalFrontier
{
    public class GameServer : BaseGame
    {
        public static readonly string WorldSeed = Guid.NewGuid().ToString();

        public GalaxyGenerator GalaxyGenerator;
        public NetworkServer NetworkServer;
        public ServerWorldManager ServerWorldManager;

        public TexturePackerAtlasData SpriteAtlasData;

        public Registry Registry;

        public Group PhysicsGroup;
        public Group ColonyGroup;
        public Group ShipGroup;
        public Group TurretGroup;
        public Group ProjectileGroup;
        public Group ShieldGroup;

        public Group HumanGroup;
        public Group AlienGroup;

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
            GameDataManager.Load();

            SpriteAtlasData = AssetManager.LoadJSON<TexturePackerAtlasData>("Textures/entity_atlas.json");

            ServerWorldManager = new ServerWorldManager(this);
            Registry = new Registry();

            PhysicsGroup = Registry.RegisterGroup<Transform, Physics>();
            ColonyGroup = Registry.RegisterGroup<Colony>();
            ShipGroup = Registry.RegisterGroup<Transform, Ship>();
            TurretGroup = Registry.RegisterGroup<Transform, Turret>();
            ProjectileGroup = Registry.RegisterGroup<Projectile>();
            ShieldGroup = Registry.RegisterGroup<Shield>();

            HumanGroup = Registry.RegisterGroup<Ship, Human>();
            AlienGroup = Registry.RegisterGroup<Ship, Alien>();

            NetworkSyncManager.Registry = Registry;
            NetworkSyncManager.LoadShared();
            NetworkSyncManager.LoadServer();

            Logging.Information("Generating galaxy...");
            var stopWatch = Stopwatch.StartNew();
            GalaxyGenerator = new GalaxyGenerator(Registry, WorldSeed, true);
            GalaxyGenerator.GenerateGalaxy();
            stopWatch.Stop();
            Logging.Information("Generated galaxy with {stars} stars in {time:0.00} ms.", GalaxyGenerator.GalaxyStars.Count, stopWatch.ElapsedMilliseconds);

            ServerWorldManager.SetupWorld();

            NetworkServer = new NetworkServer(this);
            NetworkServer.Load();

            IMGUIManager.Setup();
        }

        public override void Update(GameTimer gameTimer)
        {
            StatusSystem.RunShield(ShieldGroup, gameTimer);
            PhysicsSystem.Run(PhysicsGroup, gameTimer);
            ShipSystem.Run(ShipGroup, gameTimer);
            TurretSystem.Run(this, TurretGroup, gameTimer);
            AISystem.RunAlien(this, AlienGroup);
            ProjectileSystem.Run(this, ProjectileGroup, gameTimer);

            ServerWorldManager.Update();
            NetworkServer.Update(gameTimer);
            Registry.SystemsFinished();
            IMGUIManager.Update(gameTimer);
        }

        public override void Draw(GameTimer gameTimer)
        {
            ImGui.SetNextWindowSizeConstraints(new Vector2(200, 400), new Vector2(800, 800));
            ImGui.Begin("Players", ImGuiWindowFlags.AlwaysAutoResize);

            var loggedInPlayers = 0;
            var playingPlayers = 0;

            foreach (var player in NetworkServer.PlayerManager.Players)
            {
                if (player.IsLoggedIn)
                    loggedInPlayers += 1;
                if (player.IsPlaying)
                    playingPlayers += 1;
            }

            ImGui.Text($"Connected players: {NetworkServer.PlayerManager.Players.Count}");
            ImGui.Text($"Logged in: {loggedInPlayers}");
            ImGui.Text($"Playing: {playingPlayers}");
            ImGui.NewLine();

            foreach (var player in NetworkServer.PlayerManager.Players)
            {
                if (player.User == null)
                    continue;

                ImGui.Text(player.User.Username);
            }

            ImGui.End();

            ImGui.SetNextWindowSizeConstraints(new Vector2(200, 400), new Vector2(800, 800));
            ImGui.Begin("Tools", ImGuiWindowFlags.AlwaysAutoResize);

            //if (ImGui.Button("Spawn Alien Wave"))
            //    ServerWorldManager.SpawnAlienWave(this, 1);

            ImGui.End();

            IMGUIManager.Draw();
        }

        public override void Exit()
        {
            NetworkServer?.Dispose();
            NetworkServer = null;
        }

    } // GameServer
}
