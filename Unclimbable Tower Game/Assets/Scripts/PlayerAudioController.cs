
using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerController_FSM))]
public class PlayerAudioController : MonoBehaviour
{
    PlayerController_FSM controller;
    public GroundAudio[] audios;
    private GroundAudio currentAudio;
    private event EventHandler<GroundEventArgs> GroundChanged;
    AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        controller = GetComponent<PlayerController_FSM>();
        controller.StateChanged += OnPlayerStateChanged;

        // Setup default Audio and listener
        currentAudio = audios[0];
        GroundChanged += OnGroundChanged;
    }

    void FixedUpdate() 
    {
        // Raise an event if the player is standing on a different ground
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo) && hitInfo.transform.gameObject.layer != currentAudio.Mask)
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
        currentAudio = audios.FirstOrDefault<GroundAudio>(x => x.Mask == e.Mask) ?? currentAudio;

        // Switch continuous clip
        if (controller.CurrentState is PlayerWalkingState)
        {
            source.clip = currentAudio.WalkClip;
            source.loop = true;
            source.Play();
        }
        else if (controller.CurrentState is PlayerRunningState)
        {
            source.clip = currentAudio.RunClip;
            source.loop = true;
            source.Play();
        }
    }

    void OnPlayerStateChanged(object sender, PlayerStateChangedEventArgs e)
    {
        if (e.PreviousState is PlayerFallingState)
        {
            source.PlayOneShot(currentAudio.LandClip);
            source.loop = false;
        }
        else if (e.NewState is PlayerIdleState)
        {
            source.loop = false;
        }
        else if (e.NewState is PlayerWalkingState)
        {
            source.clip = currentAudio.WalkClip;
            source.loop = true;
            source.Play();
        }
        else if (e.NewState is PlayerRunningState)
        {
            source.clip = currentAudio.RunClip;
            source.loop = true;
            source.Play();
        }
        else if (e.NewState is PlayerJumpingState)
        {
            source.PlayOneShot(currentAudio.JumpClip);
            source.loop = false;
        }
    }

    [System.Serializable]
    public class GroundAudio
    {
        public AudioClip RunClip;
        public AudioClip WalkClip;
        public AudioClip JumpClip;
        public AudioClip LandClip;
        public int Mask;
    }

    private class GroundEventArgs : EventArgs
    {
        public int Mask;
    }

}





