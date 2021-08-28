using ElementEngine;
using ElementEngine.ElementUI;
using Veldrid;

using Rectangle = ElementEngine.Rectangle;

namespace FinalFrontier
{
    public static class UIBuilderMenu
    {
        public static UIScreen Build()
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
