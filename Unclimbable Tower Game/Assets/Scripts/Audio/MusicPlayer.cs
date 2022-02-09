// Play Music from an audio source component
// Volume changes with the game current settings

using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour {
    AudioSource _source;
    SaveSystem _saveSystem;
    const string _saveSystemName = "SaveSystem";

    void Awake() {
        _source = GetComponent<AudioSource>();

        // Save System for volume (only works if a SaveSystem is in the scene)
        _saveSystem = GameObject.Find(_saveSystemName).GetComponent<SaveSystem>();
        _saveSystem.GameSettingLoaded += OnGameSettingChanged;
        _saveSystem.GameLoaded += OnGameSettingChanged;
    }

    void OnGameSettingChanged(object sender, EventArgs e) {
        _source.volume = _saveSystem.Data.MusicVolume;
    }
}
