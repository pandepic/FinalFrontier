using ElementEngine;
using ElementEngine.ElementUI;
using System;
using System.Collections.Generic;
using Veldrid;

using Rectangle = ElementEngine.Rectangle;

namespace FinalFrontier
{
    public static class UIBuilderMenu
    {
        public static UIScreen Build(GameStateMenu menu)
        {
            var screen = new UIScreen();

            var background = new UIImage("Background", new UIImageStyle(new UISpriteStatic("Backgrounds/menu_bg.png"))
            {
                IgnoreParentPadding = true,
                UISize = new UISize() { ParentWidth = true, ParentHeight = true, FillType = UISizeFillType.Cover },
            });
            screen.AddChild(background);

            var title = new UILabel("Title", UITheme.TitleLabelStyle, LocalisationManager.GetString("Title"));
            screen.AddChild(title);

            BuildLoginContainer(screen);

            var menuButtonContainerStyle = new UIContainerStyle(new UISpriteColor(RgbaByte.Clear))
            {
                UIPosition = new UIPosition() { AnchorBottom = true, AnchorRight = true },
                UISize = new UISize() { AutoWidth = true, AutoHeight = true },
                Padding = new UISpacing(25),
            };

            var menuButtonContainer = new UIContainer("MenuButtonContainer", menuButtonContainerStyle);
            screen.AddChild(menuButtonContainer);

            var menuButtons = new List<(string Name, string Label, Action<UIOnClickArgs> Callback)>()
            {
                ("Settings", LocalisationManager.GetString("Settings"), menu.Settings_OnClick),
                ("Exit", LocalisationManager.GetString("Exit"), menu.Exit_OnClick),
            };

            foreach (var (name, label, callback) in menuButtons)
            {
                var button = new UIButton(name, UITheme.BaseWideButtonStyle);
                var buttonLabel = new UILabel($"{name}Label", UITheme.BaseButtonLabelStyle, label);

                button.SetPosition(0, 0);
                button.MarginBottom = 5;
                button.OnClick += callback;

                button.AddChild(buttonLabel);
                menuButtonContainer.AddChild(button);
            }

            return screen;
        }

        public static void BuildLoginContainer(UIScreen screen)
        {
            var loginContainerStyle = new UIContainerStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("login_box.png")));
            var loginContainer = new UIContainer("LoginContainer", loginContainerStyle);
            loginContainer.SetPadding(25);
            loginContainer.Center();
            screen.AddChild(loginContainer);

            var loginTitleStyle = new UILabelStyle(UITheme.FontRoboto, RgbaByte.White, 32, 1, UIFontStyle.Normal, UIFontWeight.Black);
            var loginTitle = new UILabel("LoginTitle", loginTitleStyle, LocalisationManager.GetString("Login"));
            loginTitle.IgnoreParentPadding = true;
            loginTitle.CenterX = true;
            loginTitle.Y = 20;
            loginContainer.AddChild(loginTitle);
        }
    } // UIBuilderMenu
}
