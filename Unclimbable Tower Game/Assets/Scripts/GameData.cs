// Class that contains all the data saved

using UnityEngine;

[System.Serializable]
public class GameData {
    public int Level;
    public int ScreenMode;
    public int Width, Height;
    public float MouseSensitivity;
    public float MusicVolume;
    public float SoundVolume;
    public GameData(GameSetting setting, int level) {
        this.Level = level;
        this.Width = setting.ScreenResolution.width;
        this.Height = setting.ScreenResolution.height;
        this.ScreenMode = (int)setting.ScreenMode;
        this.MouseSensitivity = setting.MouseSensitivity;
        this.MusicVolume = setting.MusicVolume;
        this.SoundVolume = setting.SoundVolume;
    }

    // Return the current setting
    public GameSetting GetSetting() {
        GameSetting setting = new GameSetting();
        setting.MouseSensitivity = this.MouseSensitivity;
        setting.ScreenMode = (FullScreenMode)ScreenMode;
        setting.ScreenResolution = new ScreenResolution(this.Width, this.Height);
        setting.SoundVolume = this.SoundVolume;
        setting.MusicVolume = this.MusicVolume;
        return setting;
    }

    public override string ToString() {
        return $"Level {Level}, ScreenMode {ScreenMode}, ScreenResolution {Width}x{Height}, MouseSensitivity {MouseSensitivity}, Music {MusicVolume}, Sound {SoundVolume}.";
    }
}
