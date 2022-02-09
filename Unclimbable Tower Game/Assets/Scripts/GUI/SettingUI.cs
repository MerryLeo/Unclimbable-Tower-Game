using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour {
    [SerializeField] Dropdown resolutionDropdown;
    [SerializeField] Dropdown screenModeDropdown;
    [SerializeField] Slider mouseSensitivitySlider;
    [SerializeField] Slider soundVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    SaveSystem _saveSystem;
    ScreenResolution[] _dropDownMenuResolutions; // The Resolutions MUST match the dropdown resolutions
    const string _saveSystemName = "SaveSystem";
    void Awake() {
        _saveSystem = GameObject.Find(_saveSystemName).GetComponent<SaveSystem>();
        _dropDownMenuResolutions = new ScreenResolution[4] {
            new ScreenResolution(640, 480), 
            new ScreenResolution(1280, 720),
            new ScreenResolution(1920, 1080),
            new ScreenResolution(2560, 1440)
        };
        resolutionDropdown.onValueChanged.AddListener(delegate{ OnResolutionChanged(resolutionDropdown.value); });
        mouseSensitivitySlider.onValueChanged.AddListener(delegate{ OnSensitivityChanged(mouseSensitivitySlider.value); });
        screenModeDropdown.onValueChanged.AddListener(delegate{ OnScreenModeChanged(screenModeDropdown.value); });
        soundVolumeSlider.onValueChanged.AddListener(delegate{ OnSoundVolumeChanged(soundVolumeSlider.value); });
        musicVolumeSlider.onValueChanged.AddListener(delegate{ OnMusicVolumeChanged(musicVolumeSlider.value); });
    }

    // Update UI when Opened
    void OnEnable() {
        UpdateUI();
    }

    // Save when closed
    void OnDisable() {
        SaveSystem.SaveGame(_saveSystem.Data);
    }

    void UpdateUI() {
        mouseSensitivitySlider.value = _saveSystem.Data.MouseSensitivity;
        screenModeDropdown.value = _saveSystem.Data.ScreenMode;
        int screenResIndex = 0;
        for (int i = 0; i < _dropDownMenuResolutions.Length; i++) {
            if (_dropDownMenuResolutions[i].width == _saveSystem.Data.Width && _dropDownMenuResolutions[i].height == _saveSystem.Data.Height) {
                screenResIndex = i;
                break;
            }
        }
        resolutionDropdown.value = screenResIndex;
        soundVolumeSlider.value = _saveSystem.Data.SoundVolume;
        musicVolumeSlider.value = _saveSystem.Data.MusicVolume;
    }

    public void ResetSetting() {
        _saveSystem.UpdateSetting(_saveSystem.DefaultData.GetSetting());
        UpdateUI();
    }

    public void OnResolutionChanged(int index) {
        ScreenResolution newRes = _dropDownMenuResolutions[index];
        GameSetting newSetting = new GameSetting();
        newSetting.ScreenResolution = newRes;
        newSetting.MouseSensitivity = _saveSystem.Data.MouseSensitivity;
        newSetting.ScreenMode = (FullScreenMode)_saveSystem.Data.ScreenMode;
        newSetting.SoundVolume = _saveSystem.Data.SoundVolume;
        newSetting.MusicVolume = _saveSystem.Data.MusicVolume;
        _saveSystem.UpdateSetting(newSetting);
    }

    public void OnScreenModeChanged(int index) {
        GameSetting newSetting = new GameSetting();
        newSetting.ScreenMode = (index < 2) ? (FullScreenMode)index : (FullScreenMode)(index + 1);
        newSetting.ScreenResolution = new ScreenResolution(_saveSystem.Data.Width, _saveSystem.Data.Height);
        newSetting.MouseSensitivity = _saveSystem.Data.MouseSensitivity;
        newSetting.SoundVolume = _saveSystem.Data.SoundVolume;
        newSetting.MusicVolume = _saveSystem.Data.MusicVolume;
        _saveSystem.UpdateSetting(newSetting);
    }

    public void OnSensitivityChanged(float value) {
        GameSetting newSetting = new GameSetting();
        newSetting.ScreenResolution = new ScreenResolution(_saveSystem.Data.Width, _saveSystem.Data.Height);
        newSetting.MouseSensitivity = value;
        newSetting.ScreenMode = (FullScreenMode)_saveSystem.Data.ScreenMode;
        newSetting.SoundVolume = _saveSystem.Data.SoundVolume;
        newSetting.MusicVolume = _saveSystem.Data.MusicVolume;
        _saveSystem.UpdateSetting(newSetting);
    }

    public void OnMusicVolumeChanged(float value) {
        GameSetting newSetting = new GameSetting();
        newSetting.ScreenResolution = new ScreenResolution(_saveSystem.Data.Width, _saveSystem.Data.Height);
        newSetting.MouseSensitivity = _saveSystem.Data.MouseSensitivity;
        newSetting.ScreenMode = (FullScreenMode)_saveSystem.Data.ScreenMode;
        newSetting.SoundVolume = _saveSystem.Data.SoundVolume;
        newSetting.MusicVolume = value;
        _saveSystem.UpdateSetting(newSetting);
    }

    public void OnSoundVolumeChanged(float value) {
        GameSetting newSetting = new GameSetting();
        newSetting.ScreenResolution = new ScreenResolution(_saveSystem.Data.Width, _saveSystem.Data.Height);
        newSetting.MouseSensitivity = _saveSystem.Data.MouseSensitivity;
        newSetting.ScreenMode = (FullScreenMode)_saveSystem.Data.ScreenMode;
        newSetting.SoundVolume = value;
        newSetting.MusicVolume = _saveSystem.Data.MusicVolume;
        _saveSystem.UpdateSetting(newSetting);
    }
}
