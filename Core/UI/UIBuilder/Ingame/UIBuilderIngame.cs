using Veldrid;
using ElementEngine;
using ElementEngine.ElementUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rectangle = ElementEngine.Rectangle;
using FinalFrontier.Components;
using FinalFrontier.GameData;
using FinalFrontier.Networking;
using ElementEngine.Timer;

namespace FinalFrontier
{
    public static class UIBuilderIngame
    {
        public static UIScreen UIScreen;
        public static UILabelStyle ChatMessageLabelStyle;
        public static UIContainer ChatMessageContainer;
        public static UIContainer InventoryContainer;

        public static List<(string Name, ShipComponentType? ComponentType)> InventoryGroups = new List<(string Name, ShipComponentType? ComponentType)>()
        {
            ("Weapons", null),
            ("Engines", ShipComponentType.Engine),
            ("Shields", ShipComponentType.Shield),
            ("Armour", ShipComponentType.Armour),
        };

        public static UIScreen Build(GameStatePlay play)
        {
            var screen = new UIScreen();
            UIScreen = screen;

            BuildChat(screen);
            BuildInventory(screen);

            return screen;

        } // Build

        public static void BuildChat(UIScreen screen)
        {
            ChatMessageLabelStyle = new UILabelStyle(UITheme.BaseLabelStyle)
            {
                UIPosition = new UIPosition() { Position = new Vector2I(0) },
                Margins = new UISpacing(0, 5),
                WordWrapWidth = 450,
            };

            var chatContainerStyle = new UIContainerStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("text_container.png")), fullDraggableRect: true);
            var chatContainer = new UIContainer("ChatContainer", chatContainerStyle);
            chatContainer.SetPosition(25, 300);

            var chatIconStyle = new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("chat_bubble_symbol.png")))
            {
                UIPosition = new UIPosition() { AnchorBottom = true, Position = new Vector2I(0) },
            };

            var chatIcon = new UIImage("ChatIcon", chatIconStyle);
            chatContainer.AddChild(chatIcon);

            var chatTextboxStyle = new UITextboxStyle(UITheme.BaseTextboxLabelStyle, new UISpriteStatic(Globals.UIAtlas.GetUITexture("type_box.png")))
            {
                UIPosition = new UIPosition() { AnchorBottom = true, AnchorRight = true },
                Padding = new UISpacing(10, 0),
            };

            var chatTextbox = new UITextbox("ChatTextbox", chatTextboxStyle, "");
            chatContainer.AddChild(chatTextbox);

            var chatContentContainerStyle = new UIContainerStyle(new UISpriteColor(RgbaByte.Clear), scrollbarV: UITheme.ContainerScrollbarVStyle)
            {
                UISize = new UISize() { Size = new Vector2I(447, 140) },
                UIPosition = new UIPosition() { Position = new Vector2I(2, 2) },
                Padding = new UISpacing(5),
                OverflowType = OverflowType.Scroll,
            };

            ChatMessageContainer = new UIContainer("ChatContentContainer", chatContentContainerStyle);
            ChatMessageContainer.IgnoreMouseClicks = true;
            chatContainer.AddChild(ChatMessageContainer);

            screen.AddChild(chatContainer);

        } // BuildChat

        public static void AddChatMessage(string message)
        {
            if (ChatMessageContainer.Children.Count > 100)
                ChatMessageContainer.ClearChildren();

            var scrollToBottom = ChatMessageContainer.ScrollbarV.CurrentValue == ChatMessageContainer.ScrollbarV.MaxValue;

            var messageLabel = new UILabel("", ChatMessageLabelStyle, message);
            ChatMessageContainer.AddChild(messageLabel);
            ChatMessageContainer.UpdateLayout();

            if (scrollToBottom)
                ChatMessageContainer.ScrollToBottom();
        }

        public static void BuildInventory(UIScreen screen)
        {
            var inventoryContainerStyle = new UIContainerStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("inventory_box_merge1.png")));
            InventoryContainer = new UIContainer("InventoryContainer", inventoryContainerStyle);
            InventoryContainer.Center();

            var innerInventoryContainerStyle = new UIContainerStyle(new UISpriteColor(RgbaByte.Clear), scrollbarV: UITheme.ContainerScrollbarVStyle)
            {
                UISize = new UISize() { Size = new Vector2I(502, 403) },
                UIPosition = new UIPosition() { Position = new Vector2I(30, 100) },
                Padding = new UISpacing(5),
                OverflowType = OverflowType.Scroll,
            };

            var inventoryTabButtonStyle = new UIButtonStyle(
                spriteNormal: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("inv_tab_n.png"))),
                spritePressed: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("inv_tab_p.png"))),
                spriteHover: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("inv_tab_h.png"))),
                spriteSelected: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("inv_tab_s.png"))));

            var inventoryTabGroup = new UIButtonTabGroup("InventoryTabs");
            
            inventoryTabGroup.OnValueChanged += (args) =>
            {
                foreach (var (name, type) in InventoryGroups)
                {
                    var container = InventoryContainer.FindChildByName<UIContainer>(name + "Container", true);
                    container?.HideDisable();
                }

                var show = InventoryGroups.Find((tab) => ("btn" + tab.Name) == args.CurrentTab.Name);
                InventoryContainer?.FindChildByName<UIContainer>(show.Name + "Container", true).ShowEnable();
            };

            foreach (var (name, type) in InventoryGroups)
            {
                var tabContainer = new UIContainer(name + "Container", innerInventoryContainerStyle);
                tabContainer.HideDisable();
                InventoryContainer.AddChild(tabContainer);

                var tabButton = new UIButton("btn" + name, inventoryTabButtonStyle, inventoryTabGroup);
                tabButton.SetPosition(30, 70);
                tabButton.MarginRight = 5;

                var tabButtonLabel = new UILabel("", UITheme.BaseButtonLabelStyle, name);
                tabButton.AddChild(tabButtonLabel);

                InventoryContainer.AddChild(tabButton);
            }

            var inventoryCreditsContainerStyle = new UIContainerStyle(new UISpriteColor(RgbaByte.Clear))
            {
                UISize = new UISize() { Size = new Vector2I(502, 45) },
                UIPosition = new UIPosition() { Position = new Vector2I(30, 510) },
            };

            var creditsContainer = new UIContainer("CreditsContainer", inventoryCreditsContainerStyle);
            InventoryContainer.AddChild(creditsContainer);

            var closeInventory = new UIButton("CloseInventory", UITheme.BaseCloseButtonStyle);
            closeInventory.IgnoreParentPadding = true;
            closeInventory.Y = 8;
            closeInventory.AnchorRight = true;
            closeInventory.MarginRight = 30;
            InventoryContainer.AddChild(closeInventory);

            closeInventory.OnClick += (args) =>
            {
                InventoryContainer?.HideDisable();
            };

            InventoryContainer.HideDisable();
            screen.AddChild(InventoryContainer);
        }

        public static void UpdateInventory()
        {
            ref var ship = ref ClientGlobals.PlayerShip.GetComponent<Ship>();
            ref var inventory = ref ClientGlobals.PlayerShip.GetComponent<Inventory>();

            foreach (var (name, type) in InventoryGroups)
            {
                var groupContainer = InventoryContainer.FindChildByName<UIContainer>(name + "Container", true);
                groupContainer?.ClearChildren();

                UpdateInventorySection(groupContainer, type, ref ship, ref inventory);
            }

        } // UpdateInventory

        public static void UpdateInventorySection(UIContainer groupContainer, ShipComponentType? section, ref Ship ship, ref Inventory inventory)
        {
            if (!section.HasValue)
            {
                var shipData = GameDataManager.Ships[ship.ShipType];

                foreach (var (slot, weapon) in ship.ShipWeaponData)
                {
                    var turretClass = shipData.Turrets[slot].Class;
                    var weaponData = new ShipWeaponData(weapon, turretClass);
                    CreateInventoryWeapon(groupContainer, $"{weapon.Quality} {turretClass} {weaponData.DamageType} {weaponData.ProjectileData.Type} [{weaponData.DPS:0.00} DPS]", slot, turretClass, shipData, "");
                }

                foreach (var item in inventory.Items)
                {
                    if (item.ComponentType != null)
                        continue;

                    var weaponSlotData = new ShipWeaponSlotData()
                    {
                        Slot = 0,
                        Seed = item.Seed,
                        Quality = item.Quality,
                    };

                    var weaponData = new ShipWeaponData(weaponSlotData, item.ClassType.Value);
                    CreateInventoryWeapon(groupContainer, $"{item.Quality} {item.ClassType.Value} {weaponData.DamageType} {weaponData.ProjectileData.Type} [{weaponData.DPS:0.00} DPS]", null, item.ClassType.Value, shipData, item.Seed);
                }
            }
            else
            {
                var equipped = ship.ShipComponentData[section.Value];

                ShipComponentData itemData = section.Value switch
                {
                    ShipComponentType.Engine => new ShipEngineData(equipped),
                    ShipComponentType.Shield => new ShipShieldData(equipped),
                    ShipComponentType.Armour => new ShipArmourData(equipped),
                    _ => throw new NotImplementedException(),
                };

                CreateInventoryComponent(groupContainer, itemData.ToString(), true, section.Value, "");

                foreach (var item in inventory.Items)
                {
                    if (item.ComponentType != section)
                        continue;

                    var slotData = new ShipComponentSlotData()
                    {
                        Seed = item.Seed,
                        Quality = item.Quality,
                    };

                    ShipComponentData inventoryItemData = item.ComponentType.Value switch
                    {
                        ShipComponentType.Engine => new ShipEngineData(slotData),
                        ShipComponentType.Shield => new ShipShieldData(slotData),
                        ShipComponentType.Armour => new ShipArmourData(slotData),
                        _ => throw new NotImplementedException(),
                    };

                    CreateInventoryComponent(groupContainer, inventoryItemData.ToString(), false, item.ComponentType.Value, item.Seed);
                }
            }

        } // UpdateInventorySection

        public static void CreateInventoryComponent(UIContainer groupContainer, string text, bool equipped, ShipComponentType type, string seed)
        {
            var inventoryItemContainerStyle = new UIContainerStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("full_segment_bg.png")))
            {
                UIPosition = new UIPosition() { CenterX = true, Position = new Vector2I(0), },
                Margins = new UISpacing(0, 0, 0, 5),
                Padding = new UISpacing(10),
                UISize = new UISize() { AutoHeight = true },
                OverflowType = OverflowType.Hide,
            };

            if (equipped)
                text = "EQUIPPED\n" + text;

            var labelStyle = new UILabelStyle(UITheme.BaseLabelStyle)
            {
                WordWrapWidth = 450,
            };

            var itemContainer = new UIContainer("", inventoryItemContainerStyle);
            var itemLabel = new UILabel("", labelStyle, text);

            if (!equipped)
            {
                var equipButton = new UIButton("", UITheme.BaseNormalButtonStyle);
                equipButton.SetPosition(0, 50);

                var equipLabel = new UILabel("", UITheme.BaseButtonLabelStyle, "Equip");
                equipButton.AddChild(equipLabel);

                itemContainer.AddChild(equipButton);

                equipButton.OnClick += (args) =>
                {
                    ClientPacketSender.EquipComponent(type, seed);
                };
            }

            itemContainer.AddChild(itemLabel);
            groupContainer.AddChild(itemContainer);

        } // CreateInventoryComponent

        public static void CreateInventoryWeapon(UIContainer groupContainer, string text, int? equippedSlot, ClassType classType, ShipData shipData, string seed)
        {
            var inventoryItemContainerStyle = new UIContainerStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("full_segment_bg.png")))
            {
                UIPosition = new UIPosition() { CenterX = true, Position = new Vector2I(0), },
                Margins = new UISpacing(0, 0, 0, 5),
                Padding = new UISpacing(10),
                UISize = new UISize() { AutoHeight = true },
            };

            if (equippedSlot.HasValue)
                text = "EQUIPPED " + classType.ToString() + " Slot " + (equippedSlot + 1).Value.ToString() + "\n" + text;

            var labelStyle = new UILabelStyle(UITheme.BaseLabelStyle);

            var itemContainer = new UIContainer("", inventoryItemContainerStyle);
            itemContainer.IgnoreMouseWheelEvents = true;

            var itemLabel = new UILabel("", labelStyle, text);
            itemLabel.SetPosition(0, 0);
            itemContainer.AddChild(itemLabel);

            if (!equippedSlot.HasValue)
            {
                var slotsList = new List<UIDropdownListItem<int>>();

                for (var i = 0; i < shipData.Turrets.Count; i++)
                {
                    var turret = shipData.Turrets[i];

                    if (turret.Class == classType)
                        slotsList.Add(new UIDropdownListItem<int>(i + 1));
                }

                if (slotsList.Count > 0)
                {
                    var equipButton = new UIButton("", UITheme.BaseNormalButtonStyle);
                    equipButton.SetPosition(0, 25);
                    equipButton.RespectMargins = true;

                    var slotDropDown = new UIDropdownList<int>("", UITheme.BaseDropdownListStyle, slotsList);
                    slotDropDown.SetPosition(125, 25);

                    var equipLabel = new UILabel("", UITheme.BaseButtonLabelStyle, "Equip");
                    equipButton.AddChild(equipLabel);

                    itemContainer.AddChild(equipButton);
                    itemContainer.AddChild(slotDropDown);

                    equipButton.OnClick += (args) =>
                    {
                        ClientPacketSender.EquipWeapon(slotDropDown.SelectedItem.Value - 1, seed);
                    };
                }
            }

            groupContainer.AddChild(itemContainer);

        } // CreateInventoryWeapon

    } // UIBuilderIngame
}
