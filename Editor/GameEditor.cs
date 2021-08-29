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
            AssetManager.Load(EditorGlobals.AssetsRoot, LoadAssetsMode.AutoPrependDir | LoadAssetsMode.AutoFind);

            IMGUIManager.Setup();

            EditorGlobals.WorldAssetsAtlas = AssetManager.LoadTexturePackerAtlas("Textures/entity_atlas.png", "Textures/entity_atlas.json");
            EditorGlobals.WorldAssetsAtlasPtr = IMGUIManager.AddTexture(EditorGlobals.WorldAssetsAtlas.Texture);

            EditorWindows.Add(EditorWindowType.Stars, new StarsWindow());
            EditorWindows.Add(EditorWindowType.Planets, new PlanetsWindow());
            EditorWindows.Add(EditorWindowType.Moons, new MoonsWindow());
            EditorWindows.Add(EditorWindowType.Asteroids, new AsteroidsWindow());
            EditorWindows.Add(EditorWindowType.Ships, new ShipsWindow());
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
