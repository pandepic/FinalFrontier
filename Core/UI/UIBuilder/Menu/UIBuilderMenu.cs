using ElementEngine;
using ElementEngine.ElementUI;
using FinalFrontier.Networking;
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

            BuildLoginContainer(screen, menu);
            UIBuilderMenuSettings.Build(screen, menu);

            var menuButtonContainerStyle = new UIContainerStyle(UITheme.ClearInnerContainerStyle)
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

            var serverStatusLabel = new UILabel("ServerStatus", UITheme.BaseLabelStyle, LocalisationManager.GetString("ServerStatus", ("STATUS", "Offline")));
            serverStatusLabel.AnchorLeft = true;
            serverStatusLabel.AnchorBottom = true;
            serverStatusLabel.SetMargins(25, 0, 0, 25);
            screen.AddChild(serverStatusLabel);

            return screen;
        }

        public static void BuildLoginContainer(UIScreen screen, GameStateMenu menu)
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

            var loginFormContainer = new UIContainer("LoginFormContainer", UITheme.ClearInnerContainerStyle);
            loginFormContainer.Y = 50;
            loginFormContainer.ParentWidth = true;
            loginFormContainer.Height = 250;
            loginContainer.AddChild(loginFormContainer);

            var lblUsername = new UILabel("lblUsername", UITheme.BaseLabelStyle, LocalisationManager.GetString("Username"));
            lblUsername.CenterX = true;
            lblUsername.SetPosition(0, 0);
            lblUsername.MarginBottom = 5;
            loginFormContainer.AddChild(lblUsername);

            var txtUsername = new UITextbox("txtUsername", UITheme.BaseTextboxStyle, SettingsManager.GetSetting<string>("Account", "Username"));
            txtUsername.CenterX = true;
            txtUsername.SetPosition(0, 0);
            txtUsername.MarginBottom = 5;
            loginFormContainer.AddChild(txtUsername);

            var lblPassword = new UILabel("lblPassword", UITheme.BaseLabelStyle, LocalisationManager.GetString("Password"));
            lblPassword.CenterX = true;
            lblPassword.SetPosition(0, 0);
            lblPassword.MarginBottom = 5;
            loginFormContainer.AddChild(lblPassword);

            var txtPassword = new UITextbox("txtPassword", UITheme.BasePasswordTextboxStyle, "");
            txtPassword.CenterX = true;
            txtPassword.SetPosition(0, 0);
            txtPassword.MarginBottom = 5;
            loginFormContainer.AddChild(txtPassword);

            var chkRememberUsername = new UICheckbox("chkRememberUsername", UITheme.BaseCheckboxStyle, LocalisationManager.GetString("RememberUsername"));
            chkRememberUsername.CenterX = true;
            chkRememberUsername.SetPosition(0, 0);
            chkRememberUsername.MarginBottom = 5;
            loginFormContainer.AddChild(chkRememberUsername);

            if (SettingsManager.GetSetting<string>("Account", "Username").Length > 0)
                chkRememberUsername.IsChecked = true;

            var btnLogin = new UIButton("btnLogin", UITheme.BaseWideButtonStyle);
            btnLogin.CenterX = true;
            btnLogin.AnchorBottom = true;
            btnLogin.MarginBottom = 35;
            var btnLoginLabel = new UILabel("btnLoginLabel", UITheme.BaseButtonLabelStyle, LocalisationManager.GetString("Login"));
            btnLogin.AddChild(btnLoginLabel);
            loginFormContainer.AddChild(btnLogin);

            var btnRegister = new UIButton("btnRegister", UITheme.BaseWideButtonStyle);
            btnRegister.CenterX = true;
            btnRegister.AnchorBottom = true;
            var btnRegisterLabel = new UILabel("btnRegisterLabel", UITheme.BaseButtonLabelStyle, LocalisationManager.GetString("Register"));
            btnRegister.AddChild(btnRegisterLabel);
            loginFormContainer.AddChild(btnRegister);

            btnLogin.OnClick += (args) =>
            {
                if (chkRememberUsername.IsChecked)
                    SettingsManager.UpdateSetting("Account", "Username", txtUsername.Text);
                else
                    SettingsManager.UpdateSetting("Account", "Username", "");

                GameClient.SaveSettings();
                ClientPacketSender.Login(txtUsername.Text, txtPassword.Text);
                ClientGlobals.Username = txtUsername.Text;
            };

            btnRegister.OnClick += (args) =>
            {
                ClientPacketSender.Register(txtUsername.Text, txtPassword.Text);
            };

#if DEBUG
            txtUsername.Text = "Pandepic";
            txtPassword.Text = "password";
#endif
        }
    } // UIBuilderMenu
}
