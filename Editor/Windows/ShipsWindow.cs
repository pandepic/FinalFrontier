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
    public class ShipsWindow : EditorWindow
    {
        public Dictionary<string, ShipData> Ships;

        private string _newShipName = "";
        private ShipData _editingShip;
        private Vector2 _newTurretPosition;

        private IMGUIEnumCombo<ClassType> _newShipClassDropdown = new("Ship Class");

        private IMGUIEnumCombo<ClassType> _editShipClassDropdown = new("Ship Class");
        private IMGUIEnumCombo<RankType> _editShipRankDropdown = new("Rank Required");
        private IMGUIEnumCombo<ClassType> _editShipNewTurretClassDropdown = new("Turret Class");

        public ShipsWindow() : base(EditorWindowType.Ships)
        {
            Ships = AssetManager.LoadJSON<Dictionary<string, ShipData>>("Data/Ships.json");
        }

        public override void Update(GameTimer gameTimer)
        {
        }

        public override void Draw(GameTimer gameTimer)
        {
            ImGui.SetNextWindowSizeConstraints(new Vector2(200, 400), new Vector2(800, 800));
            ImGui.Begin("Ships", ImGuiWindowFlags.AlwaysAutoResize);

            ImGui.InputText("Ship Name", ref _newShipName, 200);
            _newShipClassDropdown.Draw();

            if (ImGui.Button("Add Ship") && !Ships.ContainsKey(_newShipName))
            {
                Ships.Add(_newShipName, new ShipData()
                {
                    Name = _newShipName,
                    Class = _newShipClassDropdown.SelectedValue,
                    Atlas = EditorGlobals.WorldAssetsAtlas.DataAsset,
                    Sprite = "",
                    Scale = 1f,
                });
            }

            if (ImGui.Button("Save"))
                Save();

            ImGui.NewLine();

            ShipData removeShip = null;

            foreach (var (shipName, ship) in Ships)
            {
                if (ImGui.Button($"Remove##Ship{shipName}"))
                    removeShip = ship;

                ImGui.SameLine();

                if (ImGui.Selectable($"{shipName}##Select{shipName}", _editingShip == ship))
                {
                    _editingShip = ship;
                    _editShipClassDropdown.TrySetValue(_editingShip.Class);
                    _editShipRankDropdown.TrySetValue(_editingShip.RequiredRank);
                }
            }

            if (removeShip != null)
                Ships.Remove(removeShip.Name);

            ImGui.End();

            if (_editingShip == null)
                return;

            ImGui.SetNextWindowSizeConstraints(new Vector2(200, 400), new Vector2(800, 800));
            ImGui.Begin("Edit Ship", ImGuiWindowFlags.AlwaysAutoResize);

            var openAtlasBrowser = false;

            ImGui.Text($"Ship: {_editingShip.Name}");
            _editShipClassDropdown.Draw();
            _editShipRankDropdown.Draw();
            ImGui.InputInt("Cost", ref _editingShip.Cost);
            ImGui.InputFloat("Scale", ref _editingShip.Scale);
            ImGui.InputFloat("Base Move Speed", ref _editingShip.BaseMoveSpeed);
            ImGui.InputFloat("Base Turn Speed", ref _editingShip.BaseTurnSpeed);
            ImGui.InputFloat("Base Shield", ref _editingShip.BaseShield);
            ImGui.InputFloat("Base Armour", ref _editingShip.BaseArmour);
            ImGui.InputFloat("Base Shield Regen", ref _editingShip.BaseShieldRegen);

            ImGui.InputText("Sprite##EditShip", ref _editingShip.Sprite, 200);
            ImGui.SameLine();
            if (ImGui.Button("Find Sprite"))
                openAtlasBrowser = true;

            _editingShip.Class = _editShipClassDropdown.SelectedValue;
            _editingShip.RequiredRank = _editShipRankDropdown.SelectedValue;

            ImGui.NewLine();

            #region Turrets
            ImGui.Text("Turrets");
            ImGui.NewLine();

            _editShipNewTurretClassDropdown.Draw();
            ImGui.InputFloat2("Turret Position", ref _newTurretPosition);

            if (ImGui.Button("Add Turret"))
            {
                _editingShip.Turrets.Add(new ShipTurretData()
                {
                    Class = _editShipNewTurretClassDropdown.SelectedValue,
                    Position = _newTurretPosition,
                });
            }

            ImGui.NewLine();

            ShipTurretData removeTurret = null;

            for (int i = 0; i < _editingShip.Turrets.Count; i++)
            {
                var turret = _editingShip.Turrets[i];

                if (ImGui.Button($"Remove##Turret{i}"))
                    removeTurret = turret;

                ImGui.SameLine();
                ImGui.Text($"[{turret.Class}]");
                ImGui.SameLine();
                ImGui.InputFloat2($"Position##Turret{i}", ref turret.Position);
            }

            if (removeTurret != null)
                _editingShip.Turrets.Remove(removeTurret);

            ImGui.NewLine();
            #endregion

            if (!string.IsNullOrWhiteSpace(_editingShip.Sprite))
            {
                ImGui.NewLine();
                var shipImagePos = EditorUtility.ImGuiImageFromAtlas(_editingShip.Sprite, _editingShip.Scale);

                var shipSpriteRect = EditorGlobals.WorldAssetsAtlas.GetSpriteRect(_editingShip.Sprite);
                var shipOrigin = (shipSpriteRect.SizeF / 2f) * _editingShip.Scale;
                var turretColour = ImGui.GetColorU32(new Vector4(1f, 0f, 0f, 0.5f));

                foreach (var turret in _editingShip.Turrets)
                {
                    var turretPos = shipImagePos + shipOrigin + turret.Position;
                    var turretRadius = turret.Class switch
                    {
                        ClassType.Small => 10,
                        ClassType.Medium => 15,
                        ClassType.Large => 20,
                        _ => throw new NotImplementedException(),
                    };

                    ImGui.GetWindowDrawList().AddCircleFilled(turretPos, turretRadius, turretColour);
                }
            }

            ImGui.End();

            if (openAtlasBrowser)
                ImGui.OpenPopup("Browse Ship Sprites");

            var sprite = IMGUIControls.TexturePackerBrowser("Browse Ship Sprites", EditorGlobals.WorldAssetsAtlas, new Vector2(100));
            if (sprite != null)
                _editingShip.Sprite = sprite;
        }

        public void Save()
        {
            File.WriteAllText(AssetManager.GetAssetPath("Data/Ships.json"), JsonConvert.SerializeObject(Ships, Formatting.Indented));
        }

    } // ShipsWindow
}
