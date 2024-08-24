using ElementEngine;
using ElementEngine.UI;
using FinalFrontier;
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
    public class ProjectilesWindow : EditorWindow
    {
        public Dictionary<string, ProjectileData> Projectiles;

        private string _newProjectileName = "";

        private IMGUIEnumCombo<ProjectileType> _newProjectileTypeDropdown = new IMGUIEnumCombo<ProjectileType>("Projectile Type");
        private IMGUIEnumCombo<ProjectileType> _editProjectileTypeDropdown = new IMGUIEnumCombo<ProjectileType>("Projectile Type");

        private ProjectileData _editingProjectile;

        public ProjectilesWindow() : base(EditorWindowType.Projectiles)
        {
            Projectiles = AssetManager.LoadJSON<Dictionary<string, ProjectileData>>("Data/Projectiles.json");
        }

        public override void Update(GameTimer gameTimer)
        {
        }

        public override void Draw(GameTimer gameTimer)
        {
            DrawProjectiles();
        }

        private void DrawProjectiles()
        {
            ImGui.SetNextWindowSizeConstraints(new Vector2(200, 400), new Vector2(800, 800));
            ImGui.Begin("Projectiles", ImGuiWindowFlags.AlwaysAutoResize);

            ImGui.InputText("Name", ref _newProjectileName, 200);
            _newProjectileTypeDropdown.Draw();

            if (ImGui.Button("Add Projectile"))
            {
                if (Projectiles.ContainsKey(_newProjectileName))
                    return;

                Projectiles.Add(_newProjectileName, new ProjectileData()
                {
                    Name = _newProjectileName,
                    Type = _newProjectileTypeDropdown.SelectedValue,
                    Atlas = EditorGlobals.WorldAssetsAtlas.DataAsset,
                    Sprite = "",
                    Colour = Vector4.One,
                    Scale = 1f,
                });
            }

            if (ImGui.Button("Save"))
                SaveProjectiles();

            ImGui.NewLine();

            ProjectileData removeProjectile = null;

            foreach (var (name, projectile) in Projectiles)
            {
                if (ImGui.Button($"Remove##Projectile{name}"))
                    removeProjectile = projectile;

                ImGui.SameLine();

                if (ImGui.Selectable($"{name}##Select{name}", _editingProjectile == projectile))
                {
                    _editingProjectile = projectile;
                    _editProjectileTypeDropdown.TrySetValue(projectile.Type);
                }
            }

            if (removeProjectile != null)
                Projectiles.Remove(removeProjectile.Name);

            ImGui.End();

            if (_editingProjectile == null)
                return;

            ImGui.SetNextWindowSizeConstraints(new Vector2(200, 400), new Vector2(800, 800));
            ImGui.Begin("Edit Projectile", ImGuiWindowFlags.AlwaysAutoResize);

            var openAtlasBrowser = false;

            ImGui.Text(_editingProjectile.Name);
            _editProjectileTypeDropdown.Draw();

            ImGui.InputFloat("Scale", ref _editingProjectile.Scale);
            ImGui.ColorPicker4("Colour", ref _editingProjectile.Colour);

            ImGui.InputText("Sprite##EditProjectile", ref _editingProjectile.Sprite, 200);
            ImGui.SameLine();
            if (ImGui.Button("Find Sprite"))
                openAtlasBrowser = true;

            _editingProjectile.Type = _editProjectileTypeDropdown.SelectedValue;

            ImGui.NewLine();
            EditorUtility.ImGuiImageFromAtlas(_editingProjectile.Sprite, _editingProjectile.Scale, _editingProjectile.Colour);

            ImGui.End();

            if (openAtlasBrowser)
                ImGui.OpenPopup("Browse Projectile Sprites");

            var sprite = IMGUIControls.TexturePackerBrowser("Browse Projectile Sprites", EditorGlobals.WorldAssetsAtlas, new Vector2(100));
            if (sprite != null)
                _editingProjectile.Sprite = sprite;

        } // DrawProjectiles

        public void SaveProjectiles()
        {
            File.WriteAllText(AssetManager.GetAssetPath("Data/Projectiles.json"), JsonConvert.SerializeObject(Projectiles, Formatting.Indented));
        }

    } // ProjectilesWindow
}
