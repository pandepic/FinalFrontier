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
    public class PlanetsWindow : EditorWindow
    {
        public Dictionary<string, PlanetData> Planets;

        private string _newName = "";
        private string _newSprite = "";
        private int _newWeighting = 1;
        private bool _newCanHaveRings = true;
        private bool _newIsColonisable = false;
        private float _newScale = 0.5f;

        public PlanetData EditingPlanet;

        public PlanetsWindow() : base(EditorWindowType.Planets)
        {
            Planets = AssetManager.LoadJSON<Dictionary<string, PlanetData>>("Data/Planets.json");
        }

        public override void Update(GameTimer gameTimer)
        {
        }

        public override void Draw(GameTimer gameTimer)
        {
            ImGui.SetNextWindowSizeConstraints(new Vector2(200, 400), new Vector2(800, 800));
            ImGui.Begin("Planets", ImGuiWindowFlags.AlwaysAutoResize);

            ImGui.InputText("Name##NewPlanet", ref _newName, 200);
            ImGui.InputText("Sprite##NewPlanet", ref _newSprite, 200);
            ImGui.InputInt("Weighting##NewPlanet", ref _newWeighting);
            ImGui.Checkbox("Can Have Rings##NewPlanet", ref _newCanHaveRings);
            ImGui.Checkbox("Is Colonisable##NewPlanet", ref _newIsColonisable);
            ImGui.InputFloat("Scale##NewPlanet", ref _newScale);

            if (ImGui.Button("Add Planet"))
            {
                Planets.Add(_newName, new PlanetData()
                {
                    Name = _newName,
                    Sprite = _newSprite,
                    Weighting = _newWeighting,
                    CanHaveRings = _newCanHaveRings,
                    IsColonisable = _newIsColonisable,
                    Scale = _newScale,
                });
            }

            if (ImGui.Button("Save"))
                Save();

            ImGui.NewLine();

            foreach (var (name, data) in Planets)
            {
                if (ImGui.Selectable($"{name}##EditPlanet", EditingPlanet == data))
                    EditingPlanet = data;
            }

            ImGui.End();

            if (EditingPlanet != null)
            {
                var openTextureBrowser = false;

                ImGui.Begin($"Editing Planet", ImGuiWindowFlags.AlwaysAutoResize);

                ImGui.Text(EditingPlanet.Name);

                ImGui.InputText("Sprite##EditPlanet", ref EditingPlanet.Sprite, 200);
                ImGui.SameLine();
                if (ImGui.Button("Find Texture"))
                    openTextureBrowser = true;

                ImGui.InputInt("Weighting##EditPlanet", ref EditingPlanet.Weighting);
                ImGui.Checkbox("Can Have Rings##EditPlanet", ref EditingPlanet.CanHaveRings);
                ImGui.Checkbox("Is Colonisable##NewPlanet", ref EditingPlanet.IsColonisable);
                ImGui.InputFloat("Scale##EditPlanet", ref EditingPlanet.Scale);

                ImGui.NewLine();
                EditorUtility.ImGuiImage(EditingPlanet.Sprite, new Vector2(200));

                ImGui.End();

                if (openTextureBrowser)
                    ImGui.OpenPopup("Browse Planet Textures");

                var sprite = IMGUIControls.TextureBrowser("Browse Planet Textures", new Vector2(100));
                if (sprite != null)
                    EditingPlanet.Sprite = sprite;
            }
        }

        public void Save()
        {
            File.WriteAllText(AssetManager.GetAssetPath("Data/Planets.json"), JsonConvert.SerializeObject(Planets, Formatting.Indented));
        }
    } // PlanetsWindow
}
