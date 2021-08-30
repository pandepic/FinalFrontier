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
    public class TurretsWindow : EditorWindow
    {
        public Dictionary<string, TurretData> Turrets;

        private string _newTurretName = "";
        private IMGUIEnumCombo<ClassType> _newTurretClassDropdown = new IMGUIEnumCombo<ClassType>("Turret Class");

        private IMGUIEnumCombo<ClassType> _editTurretClassDropdown = new IMGUIEnumCombo<ClassType>("Turret Class");

        private TurretData _editingTurret;

        public TurretsWindow() : base(EditorWindowType.Turrets)
        {
            Turrets = AssetManager.LoadJSON<Dictionary<string, TurretData>>("Data/Turrets.json");
        }

        public override void Update(GameTimer gameTimer)
        {
        }

        public override void Draw(GameTimer gameTimer)
        {
            DrawTurrets();
        }

        private void DrawTurrets()
        {
            ImGui.SetNextWindowSizeConstraints(new Vector2(200, 400), new Vector2(800, 800));
            ImGui.Begin("Turrets", ImGuiWindowFlags.AlwaysAutoResize);

            ImGui.InputText("Name", ref _newTurretName, 200);
            _newTurretClassDropdown.Draw();

            if (ImGui.Button("Add Turret"))
            {
                if (Turrets.ContainsKey(_newTurretName))
                    return;

                Turrets.Add(_newTurretName, new TurretData()
                {
                    Name = _newTurretName,
                    Class = _newTurretClassDropdown.SelectedValue,
                    Atlas = EditorGlobals.WorldAssetsAtlas.DataAsset,
                    Sprite = "",
                    Scale = 1f,
                });
            }

            if (ImGui.Button("Save"))
                SaveTurrets();

            ImGui.NewLine();

            TurretData removeTurret = null;

            foreach (var (name, turret) in Turrets)
            {
                if (ImGui.Button($"Remove##{name}"))
                    removeTurret = turret;

                ImGui.SameLine();

                if (ImGui.Selectable($"[{turret.Class}] {name}", _editingTurret == turret))
                {
                    _editingTurret = turret;
                    _editTurretClassDropdown.TrySetValue(turret.Class);
                }
            }

            if (removeTurret != null)
                Turrets.Remove(removeTurret.Name);

            ImGui.End();

            if (_editingTurret == null)
                return;

            ImGui.SetNextWindowSizeConstraints(new Vector2(200, 400), new Vector2(800, 800));
            ImGui.Begin("Edit Turret", ImGuiWindowFlags.AlwaysAutoResize);

            var openAtlasBrowser = false;

            ImGui.Text(_editingTurret.Name);
            _editTurretClassDropdown.Draw();
            ImGui.InputFloat2("Projectile Spawn", ref _editingTurret.ProjectileSpawnPosition);


            ImGui.InputFloat("Scale", ref _editingTurret.Scale);
            ImGui.InputText("Sprite", ref _editingTurret.Sprite, 200);
            ImGui.SameLine();
            if (ImGui.Button("Find Sprite"))
                openAtlasBrowser = true;

            _editingTurret.Class = _editTurretClassDropdown.SelectedValue;

            ImGui.NewLine();

            if (!string.IsNullOrWhiteSpace(_editingTurret.Sprite))
            {
                ImGui.NewLine();
                var imagePos = EditorUtility.ImGuiImageFromAtlas(_editingTurret.Sprite, _editingTurret.Scale);

                var spriteRect = EditorGlobals.WorldAssetsAtlas.GetSpriteRect(_editingTurret.Sprite);
                var origin = (spriteRect.SizeF / 2f) * _editingTurret.Scale;
                var spawnColour = ImGui.GetColorU32(new Vector4(1f, 0f, 0f, 0.5f));
                var projectileSpawnPos = imagePos + origin + _editingTurret.ProjectileSpawnPosition;

                ImGui.GetWindowDrawList().AddCircleFilled(projectileSpawnPos, 5, spawnColour);
            }

            ImGui.End();

            if (openAtlasBrowser)
                ImGui.OpenPopup("Browse Turret Sprites");

            var sprite = IMGUIControls.TexturePackerBrowser("Browse Turret Sprites", EditorGlobals.WorldAssetsAtlas, new Vector2(100));
            if (sprite != null)
                _editingTurret.Sprite = sprite;

        } // DrawTurrets

        public void SaveTurrets()
        {
            File.WriteAllText(AssetManager.GetAssetPath("Data/Turrets.json"), JsonConvert.SerializeObject(Turrets, Formatting.Indented));
        }

    } // TurretsWindow
}
