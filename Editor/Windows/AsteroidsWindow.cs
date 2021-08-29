using ElementEngine;
using ElementEngine.UI;
using FinalFrontier.GameData;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public class AsteroidsWindow : EditorWindow
    {
        public Dictionary<string, AsteroidData> Asteroids;

        private string _newName = "";
        private string _newSprite = "";
        private int _newWeighting = 1;
        private float _newScale = 0.5f;

        private AsteroidData _editingAsteroid;

        public AsteroidsWindow() : base(EditorWindowType.Asteroids)
        {
            Asteroids = AssetManager.LoadJSON<Dictionary<string, AsteroidData>>("Data/Asteroids.json");
        }

        public override void Update(GameTimer gameTimer)
        {
        }

        public override void Draw(GameTimer gameTimer)
        {
            ImGui.SetNextWindowSizeConstraints(new Vector2(200, 400), new Vector2(800, 800));
            ImGui.Begin("Asteroids", ImGuiWindowFlags.AlwaysAutoResize);

            ImGui.InputText("Name##NewAsteroid", ref _newName, 200);
            ImGui.InputText("Sprite##NewAsteroid", ref _newSprite, 200);
            ImGui.InputInt("Weighting##NewAsteroid", ref _newWeighting);
            ImGui.InputFloat("Scale##NewAsteroid", ref _newScale);

            if (ImGui.Button("Add Asteroid"))
            {
                Asteroids.Add(_newName, new AsteroidData()
                {
                    Name = _newName,
                    Atlas = EditorGlobals.WorldAssetsAtlas.DataAsset,
                    Sprite = _newSprite,
                    Weighting = _newWeighting,
                    Scale = _newScale,
                });
            }

            if (ImGui.Button("Save"))
                Save();

            ImGui.NewLine();

            foreach (var (name, data) in Asteroids)
            {
                if (ImGui.Selectable($"{name}##EditAsteroid", _editingAsteroid == data))
                    _editingAsteroid = data;
            }

            ImGui.End();

            if (_editingAsteroid == null)
                return;

            var openAtlasBrowser = false;

            ImGui.Begin($"Editing Asteroid", ImGuiWindowFlags.AlwaysAutoResize);

            ImGui.Text(_editingAsteroid.Name);

            ImGui.InputText("Sprite##EditAsteroid", ref _editingAsteroid.Sprite, 200);
            ImGui.SameLine();
            if (ImGui.Button("Find Sprite"))
                openAtlasBrowser = true;

            ImGui.InputInt("Weighting##EditAsteroid", ref _editingAsteroid.Weighting);
            ImGui.InputFloat("Scale##EditAsteroid", ref _editingAsteroid.Scale);

            ImGui.NewLine();
            EditorUtility.ImGuiImageFromAtlas(_editingAsteroid.Sprite);

            ImGui.End();

            if (openAtlasBrowser)
                ImGui.OpenPopup("Browse Asteroid Sprites");

            var sprite = IMGUIControls.TexturePackerBrowser("Browse Asteroid Sprites", EditorGlobals.WorldAssetsAtlas, new Vector2(100));
            if (sprite != null)
                _editingAsteroid.Sprite = sprite;
        }

        public void Save()
        {
            File.WriteAllText(AssetManager.GetAssetPath("Data/Asteroids.json"), JsonConvert.SerializeObject(Asteroids, Formatting.Indented));
        }
    } // AsteroidsWindow
}
