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
        public static UIContainer BuyShipContainer;
        public static UIContainer InnerBuyShipContainer;

        public static string ArmourValue = "";
        public static string ShieldValue = "";

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

            var settingsButton = new UIButton("", UITheme.BaseWideButtonStyle);
            var settingsLabel = new UILabel("", UITheme.BaseButtonLabelStyle, "Settings");
            settingsButton.Y = 25;
            settingsButton.AnchorRight = true;
            settingsButton.MarginRight = 25;
            settingsButton.MarginBottom = 5;
            settingsButton.AddChild(settingsLabel);
            screen.AddChild(settingsButton);

            settingsButton.OnClick += (args) =>
            {
                var settingsContainer = UIScreen.FindChildByName<UIContainer>("SettingsContainer", true);
                UIBuilderMenuSettings.ResetSettings();
                settingsContainer.ShowEnable();
            };

            var logoutButton = new UIButton("", UITheme.BaseWideButtonStyle);
            var logoutLabel = new UILabel("", UITheme.BaseButtonLabelStyle, "Exit Game");
            logoutButton.Y = 25;
            logoutButton.AnchorRight = true;
            logoutButton.MarginRight = 25;
            logoutButton.MarginBottom = 5;
            logoutButton.AddChild(logoutLabel);
            screen.AddChild(logoutButton);

            logoutButton.OnClick += (args) =>
            {
                play.GameClient.Quit();
            };

            BuildPlayerFrame(screen);
            BuildChat(screen);
            BuildInventory(screen);
            BuildBuyShip(screen);
            UIBuilderMenuSettings.Build(screen);

            var topBarContainerStyle = new UIContainerStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("links_panel.png")));
            var topBarContainer = new UIContainer("TopbarContainer", topBarContainerStyle);
            topBarContainer.CenterX = true;

            var topBarLabelStyle = new UILabelStyle(UITheme.BaseLabelStyle);
            topBarLabelStyle.FontSize = 18;
            var topBarLabel = new UILabel("TopbarLabel", topBarLabelStyle, "");
            topBarLabel.Center();
            topBarContainer.AddChild(topBarLabel);

            screen.AddChild(topBarContainer);

            return screen;

        } // Build

        public static void BuildPlayerFrame(UIScreen screen)
        {
            var playerFrameStyle = new UIContainerStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("player_frame_base.png")), fullDraggableRect: true);
            var playerFrameContainer = new UIContainer("TopbarContainer", playerFrameStyle);
            playerFrameContainer.SetPosition(25, 25);

            var statusLabelStyle = new UILabelStyle(UITheme.BaseLabelStyle);
            statusLabelStyle.FontSize = 16;

            var playerShieldBarStyle = new UIProgressbarStyleH(
                new UIImageStyle(new UISpriteColor(RgbaByte.Clear)),
                new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("player_resource_fill_blue.png"))))
            {
                UISize = new UISize() { Size = new Vector2I(206, 14) },
            };

            var playerShieldBar = new UIProgressbarH("ShieldProgressBar", playerShieldBarStyle, 0, 100, 100);
            playerShieldBar.SetPosition(109, 17);
            var playerShieldBarLabel = new UILabel("ShieldProgressBarLabel", statusLabelStyle, "Shield");
            playerShieldBarLabel.Center();
            playerShieldBar.AddChild(playerShieldBarLabel);
            playerFrameContainer.AddChild(playerShieldBar);

            var playerArmourBarStyle = new UIProgressbarStyleH(
                new UIImageStyle(new UISpriteColor(RgbaByte.Clear)),
                new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("player_resource_fill_orange.png"))))
            {
                UISize = new UISize() { Size = new Vector2I(206, 14) },
            };

            var playerArmourBar = new UIProgressbarH("ArmourProgressBar", playerArmourBarStyle, 0, 100, 100);
            playerArmourBar.SetPosition(109, 37);
            var playerArmourBarLabel = new UILabel("ArmourProgressBarLabel", statusLabelStyle, "Armour");
            playerArmourBarLabel.Center();
            playerArmourBar.AddChild(playerArmourBarLabel);
            playerFrameContainer.AddChild(playerArmourBar);

            var playerNameLabel = new UILabel("PlayerNameLabel", statusLabelStyle, "");
            playerNameLabel.X = 15;
            playerNameLabel.Y = 25;
            playerFrameContainer.AddChild(playerNameLabel);

            screen.AddChild(playerFrameContainer);

        } // BuildPlayerFrame

        public static void UpdatePlayerFrame()
        {
            if (!ClientGlobals.PlayerShip.IsAlive)
                return;

            var playerNameLabel = UIScreen.FindChildByName<UILabel>("PlayerNameLabel", true);
            if (playerNameLabel.Text.Length == 0)
                playerNameLabel.Text = ClientGlobals.Username;

            //ref var playerShip = ref ClientGlobals.PlayerShip.GetComponent<PlayerShip>();
            ref var shield = ref ClientGlobals.PlayerShip.GetComponent<Shield>();

            var shieldPercentage = (shield.CurrentValue / shield.BaseValue) * 100f;
            var shieldString = $"{shield.CurrentValue:0} / {shield.BaseValue:0} (+{shield.RechargeRate}/s)";

            if (ShieldValue != shieldString)
            {
                ShieldValue = shieldString;
                var shieldBar = UIScreen.FindChildByName<UIProgressbarH>("ShieldProgressBar", true);
                var shieldLabel = UIScreen.FindChildByName<UILabel>("ShieldProgressBarLabel", true);

                shieldBar.CurrentValue = (int)shieldPercentage;
                shieldLabel.Text = $"{shieldString}";
            }

            ref var armour = ref ClientGlobals.PlayerShip.GetComponent<Armour>();

            var armourPercentage = (armour.CurrentValue / armour.BaseValue) * 100f;
            var armourString = $"{armour.CurrentValue:0} / {armour.BaseValue:0}";

            if (ArmourValue != armourString)
            {
                ArmourValue = armourString;
                var armourBar = UIScreen.FindChildByName<UIProgressbarH>("ArmourProgressBar", true);
                var armourLabel = UIScreen.FindChildByName<UILabel>("ArmourProgressBarLabel", true);

                armourBar.CurrentValue = (int)armourPercentage;
                armourLabel.Text = $"{armourString}";
            }

        } // UpdatePlayerFrame

        public static void BuildChat(UIScreen screen)
        {
            ChatMessageLabelStyle = new UILabelStyle(UITheme.BaseLabelStyle)
            {
                UIPosition = new UIPosition() { Position = new Vector2I(0) },
                Margins = new UISpacing(0, 5),
                WordWrapWidth = 400,
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
                Padding = new UISpacing(15, 5),
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

            var title = new UILabel("", UITheme.BaseLabelStyle, "Inventory");
            title.Y = 15;
            title.CenterX = true;
            InventoryContainer.AddChild(title);

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

            var creditsIcon = new UIImage("", new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("money_icon.png"))));
            creditsIcon.X = 15;
            creditsIcon.CenterY = true;
            creditsContainer.AddChild(creditsIcon);

            var creditsLabel = new UILabel("CreditsLabel", UITheme.BaseLabelStyle, "CREDITS");
            creditsLabel.X = 50;
            creditsLabel.CenterY = true;
            creditsContainer.AddChild(creditsLabel);

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

                var sellButton = new UIButton("", UITheme.BaseNormalButtonStyle);
                sellButton.SetPosition(370, 50);
                var sellLabel = new UILabel("", UITheme.BaseButtonLabelStyle, "Sell");
                sellButton.AddChild(sellLabel);
                itemContainer.AddChild(sellButton);

                equipButton.OnClick += (args) =>
                {
                    ClientPacketSender.EquipComponent(type, seed);
                };

                sellButton.OnClick += (args) =>
                {
                    ClientPacketSender.SellItem(seed);
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

                var sellButton = new UIButton("", UITheme.BaseNormalButtonStyle);
                sellButton.SetPosition(370, 25);
                var sellLabel = new UILabel("", UITheme.BaseButtonLabelStyle, "Sell");
                sellButton.AddChild(sellLabel);
                itemContainer.AddChild(sellButton);

                sellButton.OnClick += (args) =>
                {
                    ClientPacketSender.SellItem(seed);
                };
            }

            groupContainer.AddChild(itemContainer);

        } // CreateInventoryWeapon

        public static void BuildBuyShip(UIScreen screen)
        {
            var buyShipContainerStyle = new UIContainerStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("inventory_box_merge1.png")));
            BuyShipContainer = new UIContainer("BuyShipContainer", buyShipContainerStyle);
            BuyShipContainer.Center();

            var title = new UILabel("", UITheme.BaseLabelStyle, "Buy Ship");
            title.Y = 15;
            title.CenterX = true;
            BuyShipContainer.AddChild(title);

            var innerBuyShipContainerStyle = new UIContainerStyle(new UISpriteColor(RgbaByte.Clear), scrollbarV: UITheme.ContainerScrollbarVStyle)
            {
                UISize = new UISize() { Size = new Vector2I(502, 403) },
                UIPosition = new UIPosition() { Position = new Vector2I(30, 100) },
                Padding = new UISpacing(5),
                OverflowType = OverflowType.Scroll,
            };

            InnerBuyShipContainer = new UIContainer("InnerBuyShipContainerContainer", innerBuyShipContainerStyle);
            BuyShipContainer.AddChild(InnerBuyShipContainer);

            var inventoryCreditsContainerStyle = new UIContainerStyle(new UISpriteColor(RgbaByte.Clear))
            {
                UISize = new UISize() { Size = new Vector2I(502, 45) },
                UIPosition = new UIPosition() { Position = new Vector2I(30, 510) },
            };

            var creditsContainer = new UIContainer("CreditsContainer", inventoryCreditsContainerStyle);
            BuyShipContainer.AddChild(creditsContainer);

            var creditsIcon = new UIImage("", new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("money_icon.png"))));
            creditsIcon.X = 15;
            creditsIcon.CenterY = true;
            creditsContainer.AddChild(creditsIcon);

            var creditsLabel = new UILabel("CreditsLabel", UITheme.BaseLabelStyle, "CREDITS");
            creditsLabel.X = 50;
            creditsLabel.CenterY = true;
            creditsContainer.AddChild(creditsLabel);

            var closeBuyShip = new UIButton("CloseBuyShipContainer", UITheme.BaseCloseButtonStyle);
            closeBuyShip.IgnoreParentPadding = true;
            closeBuyShip.Y = 8;
            closeBuyShip.AnchorRight = true;
            closeBuyShip.MarginRight = 30;
            BuyShipContainer.AddChild(closeBuyShip);

            closeBuyShip.OnClick += (args) =>
            {
                BuyShipContainer?.HideDisable();
            };

            BuyShipContainer.HideDisable();
            screen.AddChild(BuyShipContainer);

        } // BuildBuyShip

        public static void UpdateBuyShip()
        {
            if (!ClientGlobals.PlayerShip.IsAlive)
                return;

            InnerBuyShipContainer.ClearChildren();

            ref var playerShip = ref ClientGlobals.PlayerShip.GetComponent<PlayerShip>();
            ref var ship = ref ClientGlobals.PlayerShip.GetComponent<Ship>();

            var buyShipItemContainerStyle = new UIContainerStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("full_segment_bg.png")))
            {
                UIPosition = new UIPosition() { CenterX = true, Position = new Vector2I(0), },
                Margins = new UISpacing(0, 0, 0, 5),
                Padding = new UISpacing(10),
                UISize = new UISize() { AutoHeight = true },
            };
            
            foreach (var (shipName, shipData) in GameDataManager.Ships)
            {
                if (shipName == ship.ShipType)
                    continue;
                if (shipData.Cost == 0)
                    continue;

                var shipText = $"{shipName} [Cost: {shipData.Cost:0}] [Rank: {shipData.RequiredRank}]";

                var labelStyle = new UILabelStyle(UITheme.BaseLabelStyle);

                var itemContainer = new UIContainer("", buyShipItemContainerStyle);
                itemContainer.IgnoreMouseWheelEvents = true;

                var itemLabel = new UILabel("", labelStyle, shipText);
                itemLabel.SetPosition(0, 0);
                itemContainer.AddChild(itemLabel);

                var buyButton = new UIButton("", UITheme.BaseNormalButtonStyle);
                buyButton.SetPosition(0, 25);

                var buyLabel = new UILabel("", UITheme.BaseButtonLabelStyle, "Buy");
                buyButton.AddChild(buyLabel);

                itemContainer.AddChild(buyButton);

                InnerBuyShipContainer.AddChild(itemContainer);

                buyButton.OnClick += (args) =>
                {
                    ClientPacketSender.BuyShip(shipName);
                };
            }

        } // UpdateBuyShip

        public static void UpdateTopbarLabel()
        {
            var label = UIScreen.FindChildByName<UILabel>("TopbarLabel", true);

            if (label == null)
                return;

            if (!ClientGlobals.PlayerShip.IsAlive)
                return;

            ref var playerShip = ref ClientGlobals.PlayerShip.GetComponent<PlayerShip>();
            var expToNext = "";

            if (playerShip.Rank != RankType.Admiral)
                expToNext = $"/{PlayerShip.RankExpRequirements[(RankType)((int)playerShip.Rank + 1)]}";

            label.Text = $"Credits: {playerShip.Money}  Exp: {playerShip.Exp}{expToNext} ({playerShip.Rank})";
        }

        public static void UpdateCredits(int credits)
        {
            var labels = UIScreen.FindChildrenByName<UILabel>("CreditsLabel", true);

            foreach (var label in labels)
                label.Text = credits.ToString();
        }

    } // UIBuilderIngame
}
