/*
    Script that manages audio for a first person player controller.
    Different Sounds are played based on: 
    - the ground the player is standing on
    - the state that the player is in
*/
using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerController_FSM))]
public class PlayerAudioController : MonoBehaviour {
    PlayerController_FSM _controller;
    GroundSound[] _sounds;
    GroundSound _currentSound;
    event EventHandler<GroundEventArgs> GroundChanged;
    AudioSource _source;
    GameManager _manager;
    SaveSystem _saveSystem;
    float volume = 1;
    const string _gameManagerName = "GameManager", _saveSystemName = "SaveSystem";
    void Awake() {
        _source = GetComponent<AudioSource>();
        _controller = GetComponent<PlayerController_FSM>();
        _controller.StateChanged += OnPlayerStateChanged;
        _saveSystem = GameObject.Find(_saveSystemName).GetComponent<SaveSystem>();
        _saveSystem.GameSettingLoaded += OnGameSettingChanged;
        _saveSystem.GameLoaded += OnGameSettingChanged;

        // Setup default audio and listener
        _sounds = GameObject.FindObjectOfType<AudioManager>().GroundAudios;
        _currentSound = _sounds[0];
        GroundChanged += OnGroundChanged;

        // Add Listener to the game manager events
        _manager = GameObject.Find(_gameManagerName).GetComponent<GameManager>();
        _manager.PauseEvent += OnGamePaused;
        _manager.WonEvent += OnGamePaused;
        _manager.GameOverEvent += OnGamePaused;
        _manager.ResumeEvent += OnGameResumed;
    }

    // Raise an event if the player is standing on a different ground
    void FixedUpdate() { 
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo) && hitInfo.transform.gameObject.layer != _currentSound.Mask) {
            EventHandler<GroundEventArgs> handler = GroundChanged;
            GroundEventArgs args = new GroundEventArgs();
            args.Mask = hitInfo.transform.gameObject.layer;
            GroundChanged?.Invoke(this, args);
        }
    }

    // Change the sound volume
    void OnGameSettingChanged(object sender, EventArgs e) {
        this.volume = _saveSystem.Data.SoundVolume;
    }

    // Stop Audio if the game is paused, the player won or lost
    void OnGamePaused(object sender, EventArgs e) {
        _source.Stop();
    }

    // Play Audio if the game is paused
    void OnGameResumed(object sender, EventArgs e) {
        PlayerStateChangedEventArgs args = new PlayerStateChangedEventArgs();
        args.PreviousState = _controller.IdleState;
        args.NewState = _controller.CurrentState;
        OnPlayerStateChanged(this, args);
    }

    // Switch Current Ground Sound when Player is Stanting on different ground
    void OnGroundChanged(object sender, GroundEventArgs e) {
        // Find audio that match the current ground
        _currentSound = _sounds.FirstOrDefault<GroundSound>(x => x.Mask == e.Mask) ?? _currentSound;

        // Switch continuous clip
        if (_controller.CurrentState is PlayerWalkingState) 
            PlaySound(_currentSound.WalkSound);
        
        else if (_controller.CurrentState is PlayerRunningState)
            PlaySound(_currentSound.RunSound);
        
    }

    // Switch Clip when Player CurrentState Changes
    void OnPlayerStateChanged(object sender, PlayerStateChangedEventArgs e) {
        if (e.PreviousState is PlayerFallingState)
            PlaySoundOnce(_currentSound.LandSound);
        
        else if (e.NewState is PlayerIdleState)
            _source.loop = false;
        
        else if (e.NewState is PlayerWalkingState)
            PlaySound(_currentSound.WalkSound);
        
        else if (e.NewState is PlayerRunningState)
            PlaySound(_currentSound.RunSound);
        
        else if (e.NewState is PlayerJumpingState)
            PlaySoundOnce(_currentSound.JumpSound);
        
    }

    // Play Continuous Sound
    void PlaySound(Sound sound) {
        _source.volume = sound.volume * this.volume;
        _source.pitch = sound.pitch;
        _source.clip = sound.clip;
        _source.loop = true;
        _source.Play();
    }

    // Play Sound Only Once
    void PlaySoundOnce(Sound sound) {
        _source.loop = false;
        _source.pitch = sound.pitch;
        _source.PlayOneShot(sound.clip, sound.volume * this.volume);
    }

    private class GroundEventArgs : EventArgs {
        public int Mask;
    }
}





