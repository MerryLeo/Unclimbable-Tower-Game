using UnityEngine;
using System.Linq;

[RequireComponent(typeof(PickableObject))]
[RequireComponent(typeof(Rigidbody))]
public class PropAudioController : MonoBehaviour
{
    AudioManager manager;
    Rigidbody rbody;
    const float minVol = 0.02f, maxVol = 1f;
    const float maxSpeedThreshold = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        rbody = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision other) 
    {
        // Play Sound from Point of Contact
        if (other != null) 
        {
            for (int i = 0; i < other.contactCount; i++)
            {
                ContactPoint contact = other.GetContact(i);
                float speed = Mathf.Abs(Vector3.Dot(rbody.velocity, (contact.point - transform.position).normalized));
                float volume = (speed <= maxSpeedThreshold) ? UtilityClass.Remap(speed, 0, maxSpeedThreshold, minVol, maxVol) : maxVol;
                PropSound sound = manager.PropAudios.FirstOrDefault<PropSound>(x => x.Mask == contact.otherCollider.gameObject.layer);
                AudioSource.PlayClipAtPoint(sound.CollisionSound.clip, contact.point, volume);
            }
        }
    }
}
