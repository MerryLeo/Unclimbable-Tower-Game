using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerController_FSM))]
public class PlayerAudioController : MonoBehaviour
{
    PlayerController_FSM controller;
    GroundSound[] sounds;
    GroundSound currentSound;
    event EventHandler<GroundEventArgs> GroundChanged;
    AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        controller = GetComponent<PlayerController_FSM>();
        controller.StateChanged += OnPlayerStateChanged;

        // Setup default audio and listener
        sounds = GameObject.FindObjectOfType<AudioManager>().GroundAudios;
        currentSound = sounds[0];
        GroundChanged += OnGroundChanged;
    }

    void FixedUpdate() 
    {
        // Raise an event if the player is standing on a different ground
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo) && hitInfo.transform.gameObject.layer != currentSound.Mask)
        {
            EventHandler<GroundEventArgs> handler = GroundChanged;
            GroundEventArgs args = new GroundEventArgs();
            args.Mask = hitInfo.transform.gameObject.layer;
            GroundChanged?.Invoke(this, args);
        }
    }

    void OnGroundChanged(object sender, GroundEventArgs e)
    {
        // Find audio that match the current ground
        currentSound = sounds.FirstOrDefault<GroundSound>(x => x.Mask == e.Mask) ?? currentSound;

        // Switch continuous clip
        if (controller.CurrentState is PlayerWalkingState)
        {
            PlaySound(currentSound.WalkSound);
        }
        else if (controller.CurrentState is PlayerRunningState)
        {
            PlaySound(currentSound.RunSound);
        }
    }

    void OnPlayerStateChanged(object sender, PlayerStateChangedEventArgs e)
    {
        if (e.PreviousState is PlayerFallingState)
        {
            PlaySoundOnce(currentSound.LandSound);
        }
        else if (e.NewState is PlayerIdleState)
        {
            source.loop = false;
        }
        else if (e.NewState is PlayerWalkingState)
        {
            PlaySound(currentSound.WalkSound);
        }
        else if (e.NewState is PlayerRunningState)
        {
            PlaySound(currentSound.RunSound);
        }
        else if (e.NewState is PlayerJumpingState)
        {
            PlaySoundOnce(currentSound.JumpSound);
        }
    }

    void PlaySound(Sound sound)
    {
        source.volume = sound.volume;
        source.pitch = sound.pitch;
        source.clip = sound.clip;
        source.loop = true;
        source.Play();
    }

    void PlaySoundOnce(Sound sound)
    {
        source.loop = false;
        source.pitch = sound.pitch;
        source.PlayOneShot(sound.clip, sound.volume);
    }

    private class GroundEventArgs : EventArgs
    {
        public int Mask;
    }

}





