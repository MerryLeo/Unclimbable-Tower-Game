using UnityEngine;
using System.Linq;

[RequireComponent(typeof(PickableObject))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class PropAudioController : MonoBehaviour
{
    PropSound propSound;
    AudioSource source;
    PickableObject objScript;
    Rigidbody rbody;
    const float minVol = 0.05f, maxVol = 1f;
    const float maxSpeedThreshold = 4f;
    const float maxDst = 10f;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        rbody = GetComponent<Rigidbody>();
        propSound = GameObject.Find("AudioManager").GetComponent<AudioManager>().PropAudios.FirstOrDefault<PropSound>(x => x.Mask == gameObject.layer);
        objScript = GetComponent<PickableObject>();
        source.loop = false;
        source.maxDistance = maxDst;
        source.spatialBlend = 1;
    }

    void OnCollisionEnter(Collision other) 
    {
        if (objScript.CurrentState is ObjectState.THROWN)
        {
            PlaySoundOnce(propSound.ThrownSound);
        }
        else
        {
            PlaySoundOnce(propSound.LandSound);
        }
    }

    void PlaySoundOnce(Sound sound)
    {
        float speed = rbody.velocity.magnitude;
        // source.volume = (speed <= maxSpeedThreshold) ? UtilityClass.Remap(speed, 0, maxSpeedThreshold, minVol, maxVol) : maxVol;
        source.pitch = sound.pitch;
        source.PlayOneShot(sound.clip);
    }

}
