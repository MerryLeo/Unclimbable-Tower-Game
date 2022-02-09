using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour {
    public GameData Data { get; private set; }
    public GameData DefaultData { get; private set; }
    public event EventHandler<GameSettingEventArgs> GameSettingLoaded;
    public event EventHandler GameLoaded;
    const string _filePath = "/gamedata.txt";
    void Start() {

        // Default Game Data
        GameSetting defaultSetting = new GameSetting();
        defaultSetting.MouseSensitivity = 0.5f;
        defaultSetting.ScreenMode = FullScreenMode.MaximizedWindow;
        defaultSetting.ScreenResolution = new ScreenResolution(1920, 1080);
        defaultSetting.SoundVolume = 0.5f;
        defaultSetting.MusicVolume = 0.5f;
        DefaultData = new GameData(defaultSetting, 1);

        // Set Data to Current Save or Default
        Data = LoadGame();
        if (Data == null) {
            Data = DefaultData;
            SaveGame(Data);
        }
        EventHandler handler = GameLoaded;
        handler?.Invoke(this, EventArgs.Empty);
        LoadSetting();
    }

    // Reset Data to its default value and Save
    public void ResetGame() {
        Data = LoadGame();
        if (Data == null)
            Data = DefaultData;
        Data.Level = DefaultData.Level;
        SaveGame(Data);
        EventHandler handler = GameLoaded;
        handler?.Invoke(this, EventArgs.Empty);
        LoadSetting();
        SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
    }

    // Update Current Save with newData
    public void UpdateSave(GameData newData) {
        Data = newData;
        SaveGame(newData);
        EventHandler handler = GameLoaded;
        handler?.Invoke(this, EventArgs.Empty);
    }

    public void LoadSetting() {
        FullScreenMode screenMode = (FullScreenMode)Data.ScreenMode;
        Screen.SetResolution(Data.Width, Data.Height, screenMode);

        // Invoke GameSettingLoaded
        EventHandler<GameSettingEventArgs> handler = GameSettingLoaded;
        GameSettingEventArgs args = new GameSettingEventArgs();
        args.Setting = Data.GetSetting();
        handler?.Invoke(this, args);
    }

    public void UpdateSetting(GameSetting setting) {
        
        // Update Data
        Data.Width = setting.ScreenResolution.width;
        Data.Height = setting.ScreenResolution.height;
        Data.MouseSensitivity = setting.MouseSensitivity;
        Data.ScreenMode = (int)setting.ScreenMode;
        Data.SoundVolume = setting.SoundVolume;
        Data.MusicVolume = setting.MusicVolume;

        // Load New Setting
        LoadSetting();
    }
    

    // Update binary file with Data
    public static void SaveGame(GameData data) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + _filePath;
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    // Update Data Variable with a binary file
    public static GameData LoadGame() {
        string path = Application.persistentDataPath + _filePath;
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();
            return data;
        } else {
            return null;
        }
    }
}

public class GameSettingEventArgs : EventArgs {
    public GameSetting Setting;
}

public class ScreenResolution {
    public int width;
    public int height;
    public ScreenResolution(int width, int height) {
        this.width = width;
        this.height = height;
    }
}

public class GameSetting {
    public  ScreenResolution ScreenResolution;
    public FullScreenMode ScreenMode;
    public float MouseSensitivity;
    public float MusicVolume;
    public float SoundVolume;
}