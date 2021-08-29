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
    public class MoonsWindow : EditorWindow
    {
        public Dictionary<string, MoonData> Moons;

        private string _newName = "";
        private string _newSprite = "";
        private int _newWeighting = 1;
        private float _newScale = 0.25f;

        public MoonData EditingMoon;

        public MoonsWindow() : base(EditorWindowType.Moons)
        {
            Moons = AssetManager.LoadJSON<Dictionary<string, MoonData>>("Data/Moons.json");
        }

        public override void Update(GameTimer gameTimer)
        {
        }

        public override void Draw(GameTimer gameTimer)
        {
            ImGui.SetNextWindowSizeConstraints(new Vector2(200, 400), new Vector2(800, 800));
            ImGui.Begin("Moons", ImGuiWindowFlags.AlwaysAutoResize);

            ImGui.InputText("Name##NewMoon", ref _newName, 200);
            ImGui.InputText("Sprite##NewMoon", ref _newSprite, 200);
            ImGui.InputInt("Weighting##NewMoon", ref _newWeighting);
            ImGui.InputFloat("Scale##NewMoon", ref _newScale);

            if (ImGui.Button("Add Moon"))
            {
                Moons.Add(_newName, new MoonData()
                {
                    Name = _newName,
                    Sprite = _newSprite,
                    Weighting = _newWeighting,
                    Scale = _newScale,
                });
            }

            if (ImGui.Button("Save"))
                Save();

            ImGui.NewLine();

            foreach (var (name, data) in Moons)
            {
                if (ImGui.Selectable($"{name}##EditMoon", EditingMoon == data))
                    EditingMoon = data;
            }

            ImGui.End();

            if (EditingMoon != null)
            {
                var openTextureBrowser = false;

                ImGui.Begin($"Editing Moon", ImGuiWindowFlags.AlwaysAutoResize);

                ImGui.Text(EditingMoon.Name);

                ImGui.InputText("Sprite##EditMoon", ref EditingMoon.Sprite, 200);
                ImGui.SameLine();
                if (ImGui.Button("Find Texture"))
                    openTextureBrowser = true;

                ImGui.InputInt("Weighting##EditMoon", ref EditingMoon.Weighting);
                ImGui.InputFloat("Scale##EditMoon", ref EditingMoon.Scale);

                ImGui.NewLine();
                EditorUtility.ImGuiImage(EditingMoon.Sprite, new Vector2(200));

                ImGui.End();

                if (openTextureBrowser)
                    ImGui.OpenPopup("Browse Moon Textures");

                var sprite = IMGUIControls.TextureBrowser("Browse Moon Textures", new Vector2(100));
                if (sprite != null)
                    EditingMoon.Sprite = sprite;
            }
        }

        public void Save()
        {
            File.WriteAllText(AssetManager.GetAssetPath("Data/Moons.json"), JsonConvert.SerializeObject(Moons, Formatting.Indented));
        }
    } // MoonsWindow
}
