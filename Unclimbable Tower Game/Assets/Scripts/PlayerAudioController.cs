using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerController_FSM))]
public class PlayerAudioController : MonoBehaviour
{
    PlayerController_FSM controller;
    public GroundAudio[] audios;
    private GroundAudio currentAudio;

    AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        controller = GetComponent<PlayerController_FSM>();
        controller.StateChanged += OnPlayerStateChanged;
        currentAudio = audios[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() 
    {
        // currentAudio = FindGroundAudio();
        RaycastHit hitInfo;
        GroundAudio audio = null;
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo))
        {
            Debug.Log($"Mask found was: {hitInfo.transform.gameObject.layer.ToString()}");

            audio = audios.FirstOrDefault<GroundAudio>(x => x.Mask == hitInfo.transform.gameObject.layer);
            if (audio != null)
            {
                
                currentAudio = audio;
            }
        }
    }

    void OnPlayerStateChanged(object sender, PlayerStateChangedEventArgs e)
    {
        if (e.PreviousState is PlayerFallingState)
        {
            source.PlayOneShot(currentAudio.LandClip);
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
            source.loop = false;
            source.PlayOneShot(currentAudio.JumpClip);
        }
    }


    [System.Serializable]
    public class GroundAudio
    {
        public AudioClip RunClip;
        public AudioClip WalkClip;
        public AudioClip JumpClip;
        public AudioClip LandClip;
        public LayerMask Mask;
    }

}





