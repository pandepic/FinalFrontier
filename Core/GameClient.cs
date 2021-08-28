using ElementEngine;

namespace FinalFrontier
{
    public class GameClient : BaseGame
    {
        public override void Load()
        {
        }

        public override void Update(GameTimer gameTimer)
        {
        }

        public override void Draw(GameTimer gameTimer)
        {
        }

        public override void Exit()
        {
            SettingsManager.Save("Settings.xml");
        }

        public static void UpdateAudioVolume()
        {
            SoundManager.SetMasterVolume(SettingsManager.GetSetting<float>("Sound", "MasterVolume"));
            SoundManager.SetVolume((int)AudioType.Music, SettingsManager.GetSetting<float>("Sound", "MusicVolume"));
            SoundManager.SetVolume((int)AudioType.SFX, SettingsManager.GetSetting<float>("Sound", "SFXVolume"));
            SoundManager.SetVolume((int)AudioType.UI, SettingsManager.GetSetting<float>("Sound", "UIVolume"));
        }

    } // GameClient
}
