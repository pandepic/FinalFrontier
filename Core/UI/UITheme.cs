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
        public static UIButtonStyle BaseCloseButtonStyle;
        #endregion

        #region Form controls
        public static UICheckboxStyle BaseCheckboxStyle;
        public static UICheckboxStyle BaseRadioButtonStyle;
        public static UITextboxStyle BaseTextboxStyle;
        public static UITextboxStyle BasePasswordTextboxStyle;
        #endregion

        #region Dropdown list
        public static UIDropdownListStyle BaseDropdownListStyle;
        #endregion

        #region Containers
        public static UIContainerStyle ClearInnerContainerStyle;
        public static UIContainerStyle ClearInnerContainerStyle_DEBUG;
        #endregion

        #region Scrollbars
        // vertical container scrollbar
        public static readonly UIScrollbarStyleV ContainerScrollbarVStyle;
        public static readonly UIImageStyle ContainerScrollbarVRailStyle;
        public static readonly UIButtonStyle ContainerScrollbarVSliderStyle;
        public static readonly UIButtonStyle ContainerScrollbarVButtonUpStyle;
        public static readonly UIButtonStyle ContainerScrollbarVButtonDownStyle;

        // horizontal container scrollbar
        public static readonly UIScrollbarStyleH ContainerScrollbarHStyle;
        public static readonly UIImageStyle ContainerScrollbarHRailStyle;
        public static readonly UIButtonStyle ContainerScrollbarHSliderStyle;
        public static readonly UIButtonStyle ContainerScrollbarHButtonLeftStyle;
        public static readonly UIButtonStyle ContainerScrollbarHButtonRightStyle;
        #endregion

        #region Settings
        public static readonly UIContainerStyle ClearScrollableContainerStyle;
        public static readonly UIContainerStyle ClearScrollableContainerStyle_DEBUG;
        public static readonly UIImageStyle ClearScrollableContainerHeadingHoloStyle;
        public static readonly UILabelStyle ClearScrollableContainerHeadingLabelStyle;
        public static readonly UIContainerStyle SettingsFullSectionContainerStyle;

        public static readonly UIScrollbarStyleH SettingsScrollbarHStyle;
        public static readonly UIImageStyle SettingsScrollbarHRailStyle;
        public static readonly UIImageStyle SettingsScrollbarHRailFillStyle;
        public static readonly UIButtonStyle SettingsScrollbarHSliderStyle;

        public static UIButtonStyle SettingsTabButtonStyle;
        public static UIPosition SettingsTabButtonIconPosition;
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

            BaseCloseButtonStyle = new UIButtonStyle(
                spriteNormal: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("close_n.png"))),
                spritePressed: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("close_p.png"))),
                spriteHover: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("close_h.png"))));
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

            BasePasswordTextboxStyle = new UITextboxStyle(new UILabelStyle(BaseTextboxLabelStyle, true) { LabelDisplayMode = LabelDisplayMode.Password }, new UISpriteStatic(Globals.UIAtlas.GetUITexture("normal_form.png")))
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

            #region Containers
            ClearInnerContainerStyle = new UIContainerStyle(new UISpriteColor(RgbaByte.Clear));
            ClearInnerContainerStyle_DEBUG = new UIContainerStyle(new UISpriteColor(RgbaByte.Red));
            #endregion

            #region Scrollbars
            // vertical container scrollbar
            ContainerScrollbarVRailStyle = new UIImageStyle(
                new UISprite3SliceVertical(
                    top: Globals.UIAtlas.GetUITexture("scrollv_rail_top.png"),
                    bottom: Globals.UIAtlas.GetUITexture("scrollv_rail_bottom.png"),
                    center: Globals.UIAtlas.GetUITexture("scrollv_rail_center.png")));

            ContainerScrollbarVSliderStyle = new UIButtonStyle(
                spriteNormal: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("scrollv_slider_n.png"))),
                spritePressed: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("scrollv_slider_n.png"))),
                spriteHover: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("scrollv_slider_h.png"))));

            ContainerScrollbarVButtonUpStyle = new UIButtonStyle(
                spriteNormal: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("scrollv_up_n.png"))),
                spritePressed: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("scrollv_up_p.png"))),
                spriteHover: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("scrollv_up_h.png"))));

            ContainerScrollbarVButtonDownStyle = new UIButtonStyle(
                spriteNormal: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("scrollv_down_n.png"))),
                spritePressed: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("scrollv_down_p.png"))),
                spriteHover: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("scrollv_down_h.png"))));

            ContainerScrollbarVStyle = new UIScrollbarStyleV(
                    rail: ContainerScrollbarVRailStyle,
                    slider: ContainerScrollbarVSliderStyle,
                    buttonUp: ContainerScrollbarVButtonUpStyle,
                    buttonDown: ContainerScrollbarVButtonDownStyle,
                    buttonType: UIScrollbarButtonType.CenterRailEdge,
                    sliderType: UIScrollbarSliderType.Contain);

            // horizontal container scrollbar
            ContainerScrollbarHRailStyle = new UIImageStyle(
                new UISprite3SliceHorizontal(
                    left: Globals.UIAtlas.GetUITexture("scrollh_rail_left.png"),
                    right: Globals.UIAtlas.GetUITexture("scrollh_rail_right.png"),
                    center: Globals.UIAtlas.GetUITexture("scrollh_rail_center.png")));

            ContainerScrollbarHSliderStyle = new UIButtonStyle(
                spriteNormal: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("scrollh_slider_n.png"))),
                spritePressed: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("scrollh_slider_n.png"))),
                spriteHover: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("scrollh_slider_h.png"))));

            ContainerScrollbarHButtonLeftStyle = new UIButtonStyle(
                spriteNormal: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("scrollh_left_n.png"))),
                spritePressed: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("scrollh_left_p.png"))),
                spriteHover: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("scrollh_left_h.png"))));

            ContainerScrollbarHButtonRightStyle = new UIButtonStyle(
                spriteNormal: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("scrollh_right_n.png"))),
                spritePressed: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("scrollh_right_p.png"))),
                spriteHover: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("scrollh_right_h.png"))));

            ContainerScrollbarHStyle = new UIScrollbarStyleH(
                    rail: ContainerScrollbarHRailStyle,
                    slider: ContainerScrollbarHSliderStyle,
                    buttonLeft: ContainerScrollbarHButtonLeftStyle,
                    buttonRight: ContainerScrollbarHButtonRightStyle,
                    buttonType: UIScrollbarButtonType.CenterRailEdge,
                    sliderType: UIScrollbarSliderType.Contain);
            #endregion

            #region Settings
            ClearScrollableContainerStyle = new UIContainerStyle(
                background: new UISpriteColor(RgbaByte.Clear),
                scrollbarV: ContainerScrollbarVStyle)
            {
                OverflowType = OverflowType.Scroll,
            };

            ClearScrollableContainerStyle_DEBUG = new UIContainerStyle(
                background: new UISpriteColor(RgbaByte.Red),
                scrollbarV: ContainerScrollbarVStyle)
            {
                OverflowType = OverflowType.Scroll,
            };

            ClearScrollableContainerHeadingHoloStyle = new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("heading_holo.png")))
            {
                IgnoreParentPadding = true,
                IgnoreOverflow = true,
                UISize = new UISize() { ParentWidth = true },
            };

            ClearScrollableContainerHeadingLabelStyle = new UILabelStyle(
                fontFamily: FontRoboto,
                color: RgbaByte.White,
                fontSize: 24,
                outline: 1,
                fontStyle: UIFontStyle.Normal,
                fontWeight: UIFontWeight.Black)
            {
                IgnoreParentPadding = true,
                IgnoreOverflow = true,
                UIPosition = new UIPosition()
                {
                    Position = new Vector2I(0, 5),
                    CenterX = true,
                }
            };

            SettingsFullSectionContainerStyle = new UIContainerStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("full_segment_bg.png")))
            {
                UIPosition = new UIPosition() { Position = new Vector2I(0, 0) },
                Margins = new UISpacing(0, 0, 0, 10),
                Padding = new UISpacing(10),
                UISize = new UISize() { ParentWidth = true, AutoHeight = true },
            };

            SettingsScrollbarHRailStyle = new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("slider_empty_frame.png")));
            SettingsScrollbarHRailFillStyle = new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("slider_fill_blue.png"))) { ScaleType = UIScaleType.Crop };

            SettingsScrollbarHSliderStyle = new UIButtonStyle(
                spriteNormal: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("slide_normal.png"))),
                spritePressed: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("slide_normal.png"))),
                spriteHover: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("slide_h.png"))));

            SettingsScrollbarHStyle = new UIScrollbarStyleH(
                    rail: SettingsScrollbarHRailStyle,
                    slider: SettingsScrollbarHSliderStyle,
                    buttonType: UIScrollbarButtonType.InsideRail,
                    sliderType: UIScrollbarSliderType.Contain,
                    railFill: SettingsScrollbarHRailFillStyle,
                    railFillPadding: 5);

            SettingsTabButtonStyle = new UIButtonStyle(
                spriteNormal: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("tab_n.png"))),
                spritePressed: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("tab_p.png"))),
                spriteHover: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("tab_h.png"))),
                spriteSelected: new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("tab_s.png"))));

            SettingsTabButtonIconPosition = new UIPosition()
            {
                Position = new Vector2I(20, 0),
                CenterY = true,
            };
            #endregion

        } // constructor

} // UITheme
}
