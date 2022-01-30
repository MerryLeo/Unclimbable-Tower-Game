// Script to play sound when an object hits something
// This script must be on the object

using UnityEngine;
using System.Linq;

[RequireComponent(typeof(PickableObject))]
[RequireComponent(typeof(Rigidbody))]
public class PropAudioController : MonoBehaviour {
    AudioManager _audioManager; // Contains different audios assosiated with a layer each
    Rigidbody _rbody;
    const float _minVol = 0.02f, _maxVol = 1f;
    const float _maxSpeedThreshold = 0.3f;
    const string _audioManagerName = "AudioManager";

    // Get Basic Components
    void Start() {
        _audioManager = GameObject.Find(_audioManagerName).GetComponent<AudioManager>();
        _rbody = GetComponent<Rigidbody>();
    }

    // Play Sound when in contact
    void OnCollisionEnter(Collision other)  {
        if (other != null) { 
            for (int i = 0; i < other.contactCount; i++) {
                ContactPoint contact = other.GetContact(i);
                float speed = Mathf.Abs(Vector3.Dot(_rbody.velocity, (contact.point - transform.position).normalized));
                float volume = (speed <= _maxSpeedThreshold) ? UtilityClass.Remap(speed, 0, _maxSpeedThreshold, _minVol, _maxVol) : _maxVol;
                PropSound sound = _audioManager.PropAudios.FirstOrDefault<PropSound>(x => x.Mask == contact.otherCollider.gameObject.layer);
                if (sound != null)
                    AudioSource.PlayClipAtPoint(sound.CollisionSound.clip, contact.point, volume);
            }
        }
    }
}
