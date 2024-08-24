using ElementEngine;
using ElementEngine.ElementUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier
{
    public static class UIBuilderMenuSettings
    {
        // general settings
        public static UIDropdownList<string> ddlLanguages;
        public static UICheckbox chkZoomToCursor;

        // video settings
        public static UIDropdownList<Resolution> ddlResolution;
        public static UICheckbox chkBorderlessFullscreen;
        public static UICheckbox chkVsync;

        // audio settings
        public static UIScrollbarH scrlMasterVolume;
        public static UIScrollbarH scrlMusicVolume;
        public static UIScrollbarH scrlSFXVolume;
        public static UIScrollbarH scrlUIVolume;

        public static void SaveSettings()
        {
            // general
            Globals.SetLanguage(ddlLanguages.SelectedItem.Value);
            SettingsManager.UpdateSetting("Gameplay", "ZoomToCursor", chkZoomToCursor.IsChecked);

            // video
            Globals.SaveResolution(ddlResolution.SelectedItem.Value);
            SettingsManager.UpdateSetting("Window", "BorderlessFullscreen", chkBorderlessFullscreen.IsChecked);
            SettingsManager.UpdateSetting("Window", "Vsync", chkVsync.IsChecked);

            // audio
            SettingsManager.UpdateSetting("Sound", "MasterVolume", scrlMasterVolume.NormalizedValue);
            SettingsManager.UpdateSetting("Sound", "MusicVolume", scrlMusicVolume.NormalizedValue);
            SettingsManager.UpdateSetting("Sound", "SFXVolume", scrlSFXVolume.NormalizedValue);
            SettingsManager.UpdateSetting("Sound", "UIVolume", scrlUIVolume.NormalizedValue);

            GameClient.UpdateAudioVolume();
        }

        public static void ResetSettings()
        {
            // general
            ddlLanguages.SetSelectedValue(SettingsManager.GetSetting<string>("UI", "Language"));
            chkZoomToCursor.IsChecked = SettingsManager.GetSetting<bool>("Gameplay", "ZoomToCursor");

            // video
            ddlResolution.SetSelectedValue(new Resolution(SettingsManager.GetSetting<int>("Window", "Width"), SettingsManager.GetSetting<int>("Window", "Height")));
            chkBorderlessFullscreen.IsChecked = SettingsManager.GetSetting<bool>("Window", "BorderlessFullscreen");
            chkVsync.IsChecked = SettingsManager.GetSetting<bool>("Window", "Vsync");

            // audio
            scrlMasterVolume.NormalizedValue = SettingsManager.GetSetting<float>("Sound", "MasterVolume");
            scrlMusicVolume.NormalizedValue = SettingsManager.GetSetting<float>("Sound", "MusicVolume");
            scrlSFXVolume.NormalizedValue = SettingsManager.GetSetting<float>("Sound", "SFXVolume");
            scrlUIVolume.NormalizedValue = SettingsManager.GetSetting<float>("Sound", "UIVolume");
        }

        public static void Build(UIScreen screen)
        {
            var settingsContainerStyle = new UIContainerStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("options_box_merge2.png")));
            var settingsContainer = new UIContainer("SettingsContainer", settingsContainerStyle);

            settingsContainer.Center();
            settingsContainer.HideDisable();
            screen.AddChild(settingsContainer);

            var settingsTopCloseButton = new UIButton("SettingsTopCloseButton", UITheme.BaseCloseButtonStyle);
            settingsTopCloseButton.IgnoreParentPadding = true;
            settingsTopCloseButton.Y = 8;
            settingsTopCloseButton.AnchorRight = true;
            settingsTopCloseButton.MarginRight = 30;

            settingsTopCloseButton.OnClick += (args) =>
            {
                settingsContainer.HideDisable();
            };

            settingsContainer.AddChild(settingsTopCloseButton);

            var settingsApplyButton = new UIButton("OptionsContainer_ApplySettings", UITheme.BaseNormalButtonStyle);
            settingsApplyButton.AnchorRight = true;
            settingsApplyButton.AnchorBottom = true;
            settingsApplyButton.MarginBottom = 50;
            settingsApplyButton.MarginRight = 50;
            var settingsApplyButtonLabel = new UILabel("OptionsContainer_ApplySettings_Label", UITheme.BaseButtonLabelStyle, LocalisationManager.GetString("Apply"));
            settingsApplyButtonLabel.Center();
            settingsApplyButton.AddChild(settingsApplyButtonLabel);
            settingsContainer.AddChild(settingsApplyButton);

            settingsApplyButton.OnClick += (args) =>
            {
                SaveSettings();
                settingsContainer.HideDisable();
            };

            var settingsCloseButton = new UIButton("OptionsContainer_CloseSettings", UITheme.BaseNormalButtonStyle);
            settingsCloseButton.AnchorLeft = true;
            settingsCloseButton.AnchorBottom = true;
            settingsCloseButton.MarginBottom = 50;
            settingsCloseButton.MarginLeft = 50;
            var settingsCloseButtonLabel = new UILabel("OptionsContainer_CloseSettings_Label", UITheme.BaseButtonLabelStyle, LocalisationManager.GetString("Close"));
            settingsCloseButtonLabel.Center();
            settingsCloseButton.AddChild(settingsCloseButtonLabel);
            settingsContainer.AddChild(settingsCloseButton);

            settingsCloseButton.OnClick += (args) =>
            {
                settingsContainer.HideDisable();
            };

            // *** BEGIN Settings Containers
            var settingsGeneralContainer = BuildGeneralSettingsContainer(LocalisationManager.GetString("GeneralSettings"));
            var settingsVideoContainer = BuildVideoSettingsContainer(LocalisationManager.GetString("VideoSettings"));
            var settingsAudioContainer = BuildAudioSettingsContainer(LocalisationManager.GetString("AudioSettings"));

            settingsContainer.AddChild(settingsGeneralContainer);
            settingsContainer.AddChild(settingsVideoContainer);
            settingsContainer.AddChild(settingsAudioContainer);
            // *** END Settings Containers

            var settingsTabContainer = new UIContainer("OptionsContainer_TabContainer", UITheme.ClearScrollableContainerStyle);
            settingsTabContainer.SetPosition(30, 70);
            settingsTabContainer.Size = new Vector2I(325, 530);
            settingsContainer.AddChild(settingsTabContainer);

            var settingsTabGroup = new UIButtonTabGroup("SettingsTabGroup");
            var settingsTabs = new List<(string Name, string Icon, string Label, UIContainer Container)>()
            {
                ("btnSettingsTabGeneral", "icon_settings.png", LocalisationManager.GetString("GeneralSettings"), settingsGeneralContainer),
                ("btnSettingsTabVideo", "icon_visuals.png", LocalisationManager.GetString("VideoSettings"), settingsVideoContainer),
                ("btnSettingsTabAudio", "icon_audio.png", LocalisationManager.GetString("AudioSettings"), settingsAudioContainer),
            };

            settingsTabGroup.OnValueChanged += (args) =>
            {
                foreach (var tab in settingsTabs)
                    tab.Container.HideDisable();

                var show = settingsTabs.Find((obj) => obj.Name == args.CurrentTab.Name);
                show.Container.ShowEnable();
            };

            foreach (var tab in settingsTabs)
            {
                var tabButton = new UIButton(tab.Name, UITheme.SettingsTabButtonStyle, settingsTabGroup);
                tabButton.Y = 5;
                tabButton.CenterX = true;
                tabButton.SetMargins(0, 0, 0, 5);

                var tabButtonLabel = new UILabel(tab.Name + "_Label", UITheme.BaseButtonLabelStyle, tab.Label);
                tabButtonLabel.Center();

                var tabButtonIcon = new UIImage(tab.Name + "_Icon", new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture(tab.Icon)))
                {
                    UIPosition = UITheme.SettingsTabButtonIconPosition,
                });

                tabButton.AddChild(tabButtonIcon);
                tabButton.AddChild(tabButtonLabel);
                settingsTabContainer.AddChild(tabButton);
            }

        } // Build

        private static UIContainer BuildSettingsContainer(string name, string title, bool hide)
        {
            var container = new UIContainer(name, UITheme.ClearScrollableContainerStyle);
            container.SetPosition(365, 70);
            container.Size = new Vector2I(540, 530);
            container.SetPadding(5, 5, 50, 5);

            if (hide)
                container.HideDisable();

            var containerTitleBG = new UIImage(name + "_TitleBG", UITheme.ClearScrollableContainerHeadingHoloStyle);
            container.AddChild(containerTitleBG);

            var containerTitleLabel = new UILabel(name + "_TitleLabel", UITheme.ClearScrollableContainerHeadingLabelStyle, title);
            container.AddChild(containerTitleLabel);

            return container;
        }

        private static UIContainer BuildGeneralSettingsContainer(string title)
        {
            var container = BuildSettingsContainer("SettingsContainer_GeneralContainer", title, false);

            var languageSection = new UIContainer("SettingsContainer_GeneralContainer_LanguageSection", UITheme.SettingsFullSectionContainerStyle);
            {
                var lblLanguage = new UILabel("lblLanguage", UITheme.BaseLabelStyle, LocalisationManager.GetString("Language"));
                lblLanguage.SetPosition(0, 0);
                lblLanguage.MarginBottom = 5;
                languageSection.AddChild(lblLanguage);

                var ddlLanguagesData = new List<UIDropdownListItem<string>>();
                foreach (var language in Globals.Languages)
                    ddlLanguagesData.Add(new UIDropdownListItem<string>(language));

                ddlLanguages = new UIDropdownList<string>("ddlLanguage", UITheme.BaseDropdownListStyle, ddlLanguagesData);
                ddlLanguages.SetPosition(0, 0);
                ddlLanguages.MarginBottom = 5;
                languageSection.AddChild(ddlLanguages);
            }
            container.AddChild(languageSection);

            var zoomToCursorSection = new UIContainer("SettingsContainer_GeneralContainer_ZoomToCursorSection", UITheme.SettingsFullSectionContainerStyle);
            {
                chkZoomToCursor = new UICheckbox("chkZoomToCursor", UITheme.BaseCheckboxStyle, LocalisationManager.GetString("ZoomToCursor"));
                zoomToCursorSection.AddChild(chkZoomToCursor);
            }
            container.AddChild(zoomToCursorSection);

            return container;

        } // BuildGeneralSettingsContainer

        private static UIContainer BuildVideoSettingsContainer(string title)
        {
            var container = BuildSettingsContainer("SettingsContainer_VideoContainer", title, true);

            //*** RESOLUTION SECTION
            var resolutionSection = new UIContainer("SettingsContainer_VideoContainer_ResolutionSection", UITheme.SettingsFullSectionContainerStyle);
            container.AddChild(resolutionSection);

            var lblResolution = new UILabel("lblResolution", UITheme.BaseLabelStyle, LocalisationManager.GetString("Resolution"));
            lblResolution.SetPosition(0, 0);
            lblResolution.MarginBottom = 5;
            resolutionSection.AddChild(lblResolution);

            var ddlResolutionsData = new List<UIDropdownListItem<Resolution>>();
            foreach (var resolution in Globals.PossibleResolutions)
                ddlResolutionsData.Add(new UIDropdownListItem<Resolution>(resolution));

            ddlResolution = new UIDropdownList<Resolution>("ddlResolution", UITheme.BaseDropdownListStyle, ddlResolutionsData);
            ddlResolution.SetPosition(0, 0);
            ddlResolution.MarginBottom = 5;
            resolutionSection.AddChild(ddlResolution);

            //*** BORDERLESS FULLSCREEN SECTION
            var borderlessSection = new UIContainer("SettingsContainer_VideoContainer_BorderlessSection", UITheme.SettingsFullSectionContainerStyle);
            container.AddChild(borderlessSection);

            chkBorderlessFullscreen = new UICheckbox("chkBorderlessFullscreen", UITheme.BaseCheckboxStyle, LocalisationManager.GetString("BorderlessFullscreen"));
            borderlessSection.AddChild(chkBorderlessFullscreen);

            //*** VSYNC FULLSCREEN SECTION
            var vsyncSection = new UIContainer("SettingsContainer_VideoContainer_VsyncSection", UITheme.SettingsFullSectionContainerStyle);
            container.AddChild(vsyncSection);

            chkVsync = new UICheckbox("chkVsync", UITheme.BaseCheckboxStyle, LocalisationManager.GetString("Vsync"));
            vsyncSection.AddChild(chkVsync);

            return container;

        } // BuildVideoSettingsContainer

        private static UIContainer BuildAudioSettingsContainer(string title)
        {
            var container = BuildSettingsContainer("SettingsContainer_AudioContainer", title, true);

            //*** MASTER VOLUME SECTION
            var masterVolumeSection = new UIContainer("SettingsContainer_AudioContainer_MasterVolumeSection", UITheme.SettingsFullSectionContainerStyle);
            container.AddChild(masterVolumeSection);

            var lblMasterVolume = new UILabel("lblMasterVolume", UITheme.BaseLabelStyle, LocalisationManager.GetString("MasterVolume"));
            lblMasterVolume.SetPosition(0, 0);
            lblMasterVolume.MarginBottom = 5;
            masterVolumeSection.AddChild(lblMasterVolume);

            scrlMasterVolume = new UIScrollbarH("scrlMasterVolume", UITheme.SettingsScrollbarHStyle, 0, 100, 1, 0);
            scrlMasterVolume.SetPosition(0, 0);
            scrlMasterVolume.MarginBottom = 5;
            scrlMasterVolume.Width = 300;
            masterVolumeSection.AddChild(scrlMasterVolume);

            scrlMasterVolume.OnValueChanged += (args) =>
            {
                SettingsManager.UpdateSetting("Sound", "MasterVolume", scrlMasterVolume.NormalizedValue.ToString());
                GameClient.UpdateAudioVolume();
            };

            //*** MUSIC VOLUME SECTION
            var musicVolumeSection = new UIContainer("SettingsContainer_AudioContainer_MusicVolumeSection", UITheme.SettingsFullSectionContainerStyle);
            container.AddChild(musicVolumeSection);

            var lblMusicVolume = new UILabel("lblMusicVolume", UITheme.BaseLabelStyle, LocalisationManager.GetString("MusicVolume"));
            lblMusicVolume.SetPosition(0, 0);
            lblMusicVolume.MarginBottom = 5;
            musicVolumeSection.AddChild(lblMusicVolume);

            scrlMusicVolume = new UIScrollbarH("scrlMusicVolume", UITheme.SettingsScrollbarHStyle, 0, 100, 1, 0);
            scrlMusicVolume.SetPosition(0, 0);
            scrlMusicVolume.MarginBottom = 5;
            scrlMusicVolume.Width = 300;
            musicVolumeSection.AddChild(scrlMusicVolume);

            scrlMusicVolume.OnValueChanged += (args) =>
            {
                SettingsManager.UpdateSetting("Sound", "MusicVolume", scrlMusicVolume.NormalizedValue.ToString());
                GameClient.UpdateAudioVolume();
            };

            //*** SFX VOLUME SECTION
            var sfxVolumeSection = new UIContainer("SettingsContainer_AudioContainer_SFXVolumeSection", UITheme.SettingsFullSectionContainerStyle);
            container.AddChild(sfxVolumeSection);

            var lblSFXVolume = new UILabel("lblSFXVolume", UITheme.BaseLabelStyle, LocalisationManager.GetString("SFXVolume"));
            lblSFXVolume.SetPosition(0, 0);
            lblSFXVolume.MarginBottom = 5;
            sfxVolumeSection.AddChild(lblSFXVolume);

            scrlSFXVolume = new UIScrollbarH("scrlSFXVolume", UITheme.SettingsScrollbarHStyle, 0, 100, 1, 0);
            scrlSFXVolume.SetPosition(0, 0);
            scrlSFXVolume.MarginBottom = 5;
            scrlSFXVolume.Width = 300;
            sfxVolumeSection.AddChild(scrlSFXVolume);

            scrlSFXVolume.OnValueChanged += (args) =>
            {
                SettingsManager.UpdateSetting("Sound", "SFXVolume", scrlSFXVolume.NormalizedValue.ToString());
                GameClient.UpdateAudioVolume();
            };

            //*** UI VOLUME SECTION
            var uiVolumeSection = new UIContainer("SettingsContainer_AudioContainer_UIVolumeSection", UITheme.SettingsFullSectionContainerStyle);
            container.AddChild(uiVolumeSection);

            var lblUIVolume = new UILabel("lblUIVolume", UITheme.BaseLabelStyle, LocalisationManager.GetString("UIVolume"));
            lblUIVolume.SetPosition(0, 0);
            lblUIVolume.MarginBottom = 5;
            uiVolumeSection.AddChild(lblUIVolume);

            scrlUIVolume = new UIScrollbarH("scrlUIVolume", UITheme.SettingsScrollbarHStyle, 0, 100, 1, 0);
            scrlUIVolume.SetPosition(0, 0);
            scrlUIVolume.MarginBottom = 5;
            scrlUIVolume.Width = 300;
            uiVolumeSection.AddChild(scrlUIVolume);

            scrlUIVolume.OnValueChanged += (args) =>
            {
                SettingsManager.UpdateSetting("Sound", "UIVolume", scrlUIVolume.NormalizedValue.ToString());
                GameClient.UpdateAudioVolume();
            };

            return container;

        } // BuildAudioSettingsContainer

    } // UIBuilderMenuSettings
}
