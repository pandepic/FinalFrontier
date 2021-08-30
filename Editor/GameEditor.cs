using ElementEngine;
using ElementEngine.UI;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Editor
{
    public enum EditorWindowType
    {
        Stars,
        Planets,
        Moons,
        Asteroids,
        Ships,
        Weapons,
        Turrets,
        Projectiles,
    }

    public class GameEditor : BaseGame
    {
        public Dictionary<EditorWindowType, EditorWindow> EditorWindows = new Dictionary<EditorWindowType, EditorWindow>();

        public override void Load()
        {
            var windowRect = new ElementEngine.Rectangle()
            {
                X = 100,
                Y = 100,
                Width = 1600,
                Height = 900
            };

            SetupWindow(windowRect, "EDITOR: Final Frontier", vsync: true, windowState: Veldrid.WindowState.Normal);
            
            Window.Resizable = false;
            ClearColor = RgbaFloat.CornflowerBlue;

            EditorGlobals.AssetsRoot = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\..\\Client\\Content").FullName;
            EditorGlobals.ServerDataRoot = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\..\\Server\\Data").FullName;
            AssetManager.Load(EditorGlobals.AssetsRoot, LoadAssetsMode.AutoPrependDir | LoadAssetsMode.AutoFind);

            IMGUIManager.Setup();

            EditorGlobals.WorldAssetsAtlas = AssetManager.LoadTexturePackerAtlas("Textures/entity_atlas.png", "Textures/entity_atlas.json");
            EditorGlobals.WorldAssetsAtlasPtr = IMGUIManager.AddTexture(EditorGlobals.WorldAssetsAtlas.Texture);

            EditorWindows.Add(EditorWindowType.Stars, new StarsWindow());
            EditorWindows.Add(EditorWindowType.Planets, new PlanetsWindow());
            EditorWindows.Add(EditorWindowType.Moons, new MoonsWindow());
            EditorWindows.Add(EditorWindowType.Asteroids, new AsteroidsWindow());
            EditorWindows.Add(EditorWindowType.Ships, new ShipsWindow());
            EditorWindows.Add(EditorWindowType.Turrets, new TurretsWindow());
            EditorWindows.Add(EditorWindowType.Projectiles, new ProjectilesWindow());
        }

        public void ToggleEditorWindow(EditorWindowType type)
        {
            if (EditorWindows.TryGetValue(type, out var window))
            {
                window.IsActive = !window.IsActive;

                if (window.IsActive)
                    window.OnShow();
            }
        }

        public override void Update(GameTimer gameTimer)
        {
            foreach (var (_, window) in EditorWindows)
            {
                if (window.IsActive)
                    window.Update(gameTimer);
            }

            IMGUIManager.Update(gameTimer);
        }

        public override void Draw(GameTimer gameTimer)
        {
            ImGui.Begin("Windows", ImGuiWindowFlags.AlwaysAutoResize);

            foreach (var (type, window) in EditorWindows)
            {
                if (ImGui.Selectable(type.ToString(), window.IsActive))
                    ToggleEditorWindow(type);
            }

            ImGui.NewLine();

            if (ImGui.Button("Copy to Server"))
            {
                File.Copy(Path.Combine(EditorGlobals.AssetsRoot, "Data/Stars.json"), Path.Combine(EditorGlobals.ServerDataRoot, "Stars.json"), true);
                File.Copy(Path.Combine(EditorGlobals.AssetsRoot, "Data/Planets.json"), Path.Combine(EditorGlobals.ServerDataRoot, "Planets.json"), true);
                File.Copy(Path.Combine(EditorGlobals.AssetsRoot, "Data/Moons.json"), Path.Combine(EditorGlobals.ServerDataRoot, "Moons.json"), true);
                File.Copy(Path.Combine(EditorGlobals.AssetsRoot, "Data/Asteroids.json"), Path.Combine(EditorGlobals.ServerDataRoot, "Asteroids.json"), true);
                File.Copy(Path.Combine(EditorGlobals.AssetsRoot, "Data/Ships.json"), Path.Combine(EditorGlobals.ServerDataRoot, "Ships.json"), true);
                File.Copy(Path.Combine(EditorGlobals.AssetsRoot, "Data/Turrets.json"), Path.Combine(EditorGlobals.ServerDataRoot, "Turrets.json"), true);
                File.Copy(Path.Combine(EditorGlobals.AssetsRoot, "Data/Projectiles.json"), Path.Combine(EditorGlobals.ServerDataRoot, "Projectiles.json"), true);
            }

            ImGui.End();

            foreach (var (_, window) in EditorWindows)
            {
                if (window.IsActive)
                    window.Draw(gameTimer);
            }

            IMGUIManager.Draw();
        }

    } // GameEditor
}
