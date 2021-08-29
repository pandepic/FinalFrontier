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
    public class StarsWindow : EditorWindow
    {
        public Dictionary<string, StarData> Stars;

        private string _newName = "";
        private string _newSprite = "";
        private int _newWeighting = 1;
        private float _newScale = 5f;

        public StarData EditingStar;

        public StarsWindow() : base(EditorWindowType.Stars)
        {
            Stars = AssetManager.LoadJSON<Dictionary<string, StarData>>("Data/Stars.json");
        }

        public override void Update(GameTimer gameTimer)
        {
        }

        public override void Draw(GameTimer gameTimer)
        {
            ImGui.SetNextWindowSizeConstraints(new Vector2(200, 400), new Vector2(800, 800));
            ImGui.Begin("Stars", ImGuiWindowFlags.AlwaysAutoResize);

            ImGui.InputText("Name##NewStar", ref _newName, 200);
            ImGui.InputText("Sprite##NewStar", ref _newSprite, 200);
            ImGui.InputInt("Weighting##NewStar", ref _newWeighting);
            ImGui.InputFloat("Scale##NewStar", ref _newScale);

            if (ImGui.Button("Add Star"))
            {
                Stars.Add(_newName, new StarData()
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

            foreach (var (name, data) in Stars)
            {
                if (ImGui.Selectable($"{name}##EditStar", EditingStar == data))
                    EditingStar = data;
            }

            ImGui.End();

            if (EditingStar != null)
            {
                var openTextureBrowser = false;

                ImGui.Begin($"Editing Star", ImGuiWindowFlags.AlwaysAutoResize);

                ImGui.Text(EditingStar.Name);

                ImGui.InputText("Sprite##EditStar", ref EditingStar.Sprite, 200);
                ImGui.SameLine();
                if (ImGui.Button("Find Texture"))
                    openTextureBrowser = true;

                ImGui.InputInt("Weighting##EditStar", ref EditingStar.Weighting);
                ImGui.InputFloat("Scale##EditStar", ref EditingStar.Scale);

                ImGui.NewLine();
                EditorUtility.ImGuiImage(EditingStar.Sprite, new Vector2(200));

                ImGui.End();

                if (openTextureBrowser)
                    ImGui.OpenPopup("Browse Star Textures");

                var sprite = IMGUIControls.TextureBrowser("Browse Star Textures", new Vector2(100));
                if (sprite != null)
                    EditingStar.Sprite = sprite;
            }
        }

        public void Save()
        {
            File.WriteAllText(AssetManager.GetAssetPath("Data/Stars.json"), JsonConvert.SerializeObject(Stars, Formatting.Indented));
        }
    } // StarsWindow
}
