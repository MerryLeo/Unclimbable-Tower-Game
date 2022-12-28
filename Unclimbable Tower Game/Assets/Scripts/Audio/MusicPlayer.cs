// Play Music from an audio source component
// Volume changes with the game current settings

using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour 
{
    AudioSource source;
    SaveSystem saveSystem;
    const string saveSystemName = "SaveSystem";

    void Awake() 
    {
        source = GetComponent<AudioSource>();

        // Save System for volume (only works if a SaveSystem is in the scene)
        saveSystem = GameObject.Find(saveSystemName).GetComponent<SaveSystem>();
        saveSystem.GameSettingLoaded += OnGameSettingChanged;
        saveSystem.GameLoaded += OnGameSettingChanged;
    }

    void OnGameSettingChanged(object sender, EventArgs e) 
    {
        source.volume = saveSystem.Data.MusicVolume;
    }
}
