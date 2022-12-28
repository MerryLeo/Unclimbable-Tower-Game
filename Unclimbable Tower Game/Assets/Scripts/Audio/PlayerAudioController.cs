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
[RequireComponent(typeof(PlayerController))]
public class PlayerAudioController : MonoBehaviour 
{
    const string GAME_MANAGER_NAME = "GameManager", SAVE_SYSTEM_NAME = "SaveSystem";
    PlayerController controller;
    GroundSound[] sounds;
    GroundSound currentSound;
    event EventHandler<GroundEventArgs> GroundChanged;
    AudioSource source;
    GameManager manager;
    SaveSystem saveSystem;
    float volume = 1;
    
    void Awake() 
    {
        source = GetComponent<AudioSource>();
        controller = GetComponent<PlayerController>();
        controller.StateChanged += OnPlayerStateChanged;
        saveSystem = GameObject.Find(SAVE_SYSTEM_NAME).GetComponent<SaveSystem>();
        saveSystem.GameSettingLoaded += OnGameSettingChanged;
        saveSystem.GameLoaded += OnGameSettingChanged;

        // Setup default audio and listener
        sounds = GameObject.FindObjectOfType<AudioManager>().GroundAudios;
        currentSound = sounds[0];
        GroundChanged += OnGroundChanged;

        // Add Listener to the game manager events
        manager = GameObject.Find(GAME_MANAGER_NAME).GetComponent<GameManager>();
        manager.PauseEvent += OnGamePaused;
        manager.WonEvent += OnGamePaused;
        manager.GameOverEvent += OnGamePaused;
        manager.ResumeEvent += OnGameResumed;
    }

    // Raise an event if the player is standing on a different ground
    void FixedUpdate() 
    { 
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo) && hitInfo.transform.gameObject.layer != currentSound.Mask)
        {
            EventHandler<GroundEventArgs> handler = GroundChanged;
            GroundEventArgs args = new GroundEventArgs();
            args.Mask = hitInfo.transform.gameObject.layer;
            GroundChanged?.Invoke(this, args);
        }
    }

    // Change the sound volume
    void OnGameSettingChanged(object sender, EventArgs e) => this.volume = saveSystem.Data.SoundVolume;

    // Stop Audio if the game is paused, the player won or lost
    void OnGamePaused(object sender, EventArgs e) => source.Stop();

    // Play Audio if the game is paused
    void OnGameResumed(object sender, EventArgs e) 
    {
        PlayerStateChangedEventArgs args = new PlayerStateChangedEventArgs();
        args.PreviousState = controller.IdleState;
        args.NewState = controller.CurrentState;
        OnPlayerStateChanged(this, args);
    }

    // Switch Current Ground Sound when Player is Stanting on different ground
    void OnGroundChanged(object sender, GroundEventArgs e) 
    {
        // Find audio that match the current ground
        currentSound = sounds.FirstOrDefault<GroundSound>(x => x.Mask == e.Mask) ?? currentSound;

        // Switch continuous clip
        if (controller.CurrentState is PlayerWalkingState) 
            PlaySound(currentSound.WalkSound);
        
        else if (controller.CurrentState is PlayerRunningState)
            PlaySound(currentSound.RunSound);
        
    }

    // Switch Clip when Player CurrentState Changes
    void OnPlayerStateChanged(object sender, PlayerStateChangedEventArgs e) 
    {
        if (e.PreviousState is PlayerFallingState)
            PlaySoundOnce(currentSound.LandSound);
        
        else if (e.NewState is PlayerIdleState)
            source.loop = false;
        
        else if (e.NewState is PlayerWalkingState)
            PlaySound(currentSound.WalkSound);
        
        else if (e.NewState is PlayerRunningState)
            PlaySound(currentSound.RunSound);
        
        else if (e.NewState is PlayerJumpingState)
            PlaySoundOnce(currentSound.JumpSound);
        
    }

    // Play Continuous Sound
    void PlaySound(Sound sound) 
    {
        source.volume = sound.volume * this.volume;
        source.pitch = sound.pitch;
        source.clip = sound.clip;
        source.loop = true;
        source.Play();
    }

    // Play Sound Only Once
    void PlaySoundOnce(Sound sound) 
    {
        source.loop = false;
        source.pitch = sound.pitch;
        source.PlayOneShot(sound.clip, sound.volume * this.volume);
    }

    private class GroundEventArgs : EventArgs 
    {
        public int Mask;
    }
}





