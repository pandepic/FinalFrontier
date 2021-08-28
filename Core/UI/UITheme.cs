using ElementEngine;
using ElementEngine.ElementUI;
using Veldrid;

using Rectangle = ElementEngine.Rectangle;

namespace FinalFrontier
{
    public static class UITheme
    {
        public static UIFontFamily FontRoboto;

        #region Labels
        public static UILabelStyle TitleLabelStyle;
        public static UILabelStyle BaseLabelStyle;
        public static UILabelStyle BaseButtonLabelStyle;
        public static UILabelStyle BaseTextboxLabelStyle;
        #endregion

        #region Buttons
        public static UIButtonStyle BaseNormalButtonStyle;
        public static UIButtonStyle BaseWideButtonStyle;
        #endregion

        #region Form controls
        public static UICheckboxStyle BaseCheckboxStyle;
        public static UICheckboxStyle BaseRadioButtonStyle;
        public static UITextboxStyle BaseTextboxStyle;
        #endregion

        #region Dropdown list
        public static UIDropdownListStyle BaseDropdownListStyle;
        #endregion

        static UITheme()
        {
            FontRoboto = new UIFontFamily("Roboto");
            FontRoboto.AddFont(UIFontStyle.Normal, UIFontWeight.Black, "Roboto/Roboto-Black.ttf");
            FontRoboto.AddFont(UIFontStyle.Italic, UIFontWeight.Black, "Roboto/Roboto-BlackItalic.ttf");
            FontRoboto.AddFont(UIFontStyle.Normal, UIFontWeight.Bold, "Roboto/Roboto-Bold.ttf");
            FontRoboto.AddFont(UIFontStyle.Italic, UIFontWeight.Bold, "Roboto/Roboto-BoldItalic.ttf");
            FontRoboto.AddFont(UIFontStyle.Italic, UIFontWeight.Normal, "Roboto/Roboto-Italic.ttf");
            FontRoboto.AddFont(UIFontStyle.Normal, UIFontWeight.Light, "Roboto/Roboto-Light.ttf");
            FontRoboto.AddFont(UIFontStyle.Italic, UIFontWeight.Light, "Roboto/Roboto-LightItalic.ttf");
            FontRoboto.AddFont(UIFontStyle.Normal, UIFontWeight.Medium, "Roboto/Roboto-Medium.ttf");
            FontRoboto.AddFont(UIFontStyle.Italic, UIFontWeight.Medium, "Roboto/Roboto-MediumItalic.ttf");
            FontRoboto.AddFont(UIFontStyle.Normal, UIFontWeight.Normal, "Roboto/Roboto-Regular.ttf");
            FontRoboto.AddFont(UIFontStyle.Normal, UIFontWeight.Thin, "Roboto/Roboto-Thin.ttf");
            FontRoboto.AddFont(UIFontStyle.Italic, UIFontWeight.Thin, "Roboto/Roboto-ThinItalic.ttf");

            #region Labels
            TitleLabelStyle = new UILabelStyle(
                fontFamily: FontRoboto,
                color: RgbaByte.White,
                fontSize: 60,
                outline: 1,
                fontStyle: UIFontStyle.Normal,
                fontWeight: UIFontWeight.Black)
            {
                UIPosition = new UIPosition()
                {
                    Position = new Vector2I(0, 50),
                    CenterX = true,
                }
            };

            BaseLabelStyle = new UILabelStyle(
                fontFamily: FontRoboto,
                color: RgbaByte.White,
                fontSize: 20,
                outline: 1,
                fontStyle: UIFontStyle.Normal,
                fontWeight: UIFontWeight.Black);

            BaseButtonLabelStyle = new UILabelStyle(BaseLabelStyle)
            {
                UIPosition = new UIPosition() { CenterX = true, CenterY = true }
            };

            BaseTextboxLabelStyle = new UILabelStyle(BaseLabelStyle)
            {
                UIPosition = new UIPosition() { CenterY = true }
            };
            #endregion

            #region Buttons
            BaseNormalButtonStyle = new UIButtonStyle(
                spriteNormal: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("button_normal_n.png"))),
                spritePressed: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("button_normal_p.png"))),
                spriteHover: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("button_normal_h.png"))),
                spriteDisabled: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("button_normal_d.png"))));

            BaseWideButtonStyle = new UIButtonStyle(
                spriteNormal: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("wide_button_n.png"))),
                spritePressed: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("wide_button_p.png"))),
                spriteHover: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("wide_button_h.png"))),
                spriteDisabled: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("wide_button_d.png"))));
            #endregion

            #region Form controls
            var checkboxTextPadding = 15;

            BaseCheckboxStyle = new UICheckboxStyle(
                textStyleNormal: BaseLabelStyle,
                textPadding: checkboxTextPadding,
                spriteUnchecked: new UISpriteStatic(Globals.UIAtlas.GetUITexture("check_n.png")),
                spriteChecked: new UISpriteStatic(Globals.UIAtlas.GetUITexture("check_ch.png")),
                spriteHover: new UISpriteStatic(Globals.UIAtlas.GetUITexture("check_h.png")));

            BaseRadioButtonStyle = new UICheckboxStyle(
                textStyleNormal: BaseLabelStyle,
                textPadding: checkboxTextPadding,
                spriteUnchecked: new UISpriteStatic(Globals.UIAtlas.GetUITexture("radio_n.png")),
                spriteChecked: new UISpriteStatic(Globals.UIAtlas.GetUITexture("radio_ch.png")),
                spriteHover: new UISpriteStatic(Globals.UIAtlas.GetUITexture("radio_h.png")));

            BaseTextboxStyle = new UITextboxStyle(BaseTextboxLabelStyle, new UISpriteStatic(Globals.UIAtlas.GetUITexture("normal_form.png")))
            {
                Padding = new UISpacing(10, 0),
            };
            #endregion

            #region Dropdown list
            var dropdownScrollbarVRailStyle = new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("dd_rail.png")));

            var dropdownScrollbarVSliderStyle = new UIButtonStyle(
                spriteNormal: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("dd_slide_n.png"))),
                spritePressed: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("dd_slide_n.png"))),
                spriteHover: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("dd_slide_h.png"))));

            var dropdownScrollbarVStyle = new UIScrollbarStyleV(
                    rail: dropdownScrollbarVRailStyle,
                    slider: dropdownScrollbarVSliderStyle,
                    buttonUp: null,
                    buttonDown: null,
                    buttonType: UIScrollbarButtonType.InsideRail,
                    sliderType: UIScrollbarSliderType.Contain);

            var dropdownLabelStyle = new UILabelStyle(BaseLabelStyle)
            {
                UIPosition = new UIPosition()
                {
                    CenterX = true,
                    CenterY = true,
                }
            };

            var dropdownSelectedLabelStyle = new UILabelStyle(BaseLabelStyle)
            {
                UIPosition = new UIPosition()
                {
                    CenterX = true,
                    CenterY = true,
                }
            };

            BaseDropdownListStyle = new UIDropdownListStyle(
                buttonCollapsed: new UIButtonStyle(
                    spriteNormal: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("dd_n.png"))),
                    spritePressed: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("dd_p.png"))),
                    spriteHover: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("dd_h.png")))),

                buttonExpanded: new UIButtonStyle(
                    spriteNormal: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("dd_n.png"))),
                    spritePressed: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("dd_p.png"))),
                    spriteHover: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("dd_h.png")))),

                listContainer: new UIContainerStyle(
                    background: new UISpriteStatic(Globals.UIAtlas.GetUITexture("drop_down_dropbg.png")),
                    scrollbarV: dropdownScrollbarVStyle)
                {
                    UISize = new UISize() { ParentWidth = true, AutoHeight = true, MaxHeight = 200 }
                },

                selectedLabelStyle: dropdownSelectedLabelStyle,

                itemButtonStyle: new UIButtonStyle(
                    spriteNormal: new UIImageStyle(
                        sprite: new UISpriteColor(RgbaByte.Clear, new Vector2I(1, 29)))
                    {
                        UISize = new UISize() { ParentWidth = true }
                    },
                    spriteHover: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("drop_down_entry_highlight.png")))
                    {
                        UISize = new UISize() { ParentWidth = true }
                    })
                {
                    UIPosition = new UIPosition() { CenterX = true, },
                    UISize = new UISize() { ParentWidth = true, },
                },

                itemButtonLabelStyle: dropdownLabelStyle);
            #endregion

        } // constructor

    } // UITheme
}
