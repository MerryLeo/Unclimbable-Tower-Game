// Respawn an object when it falls below lowerBound

using UnityEngine;

[RequireComponent(typeof(PickableObject))]
[RequireComponent(typeof(Rigidbody))]
public class RespawnOutOfBound : MonoBehaviour {
    const float LOWER_BOUND = -10f, TOP_BOUND = 10f;
    Vector3 _initialPos;
    Quaternion _initialRot;
    Rigidbody _rbody;
    
    void Start() {
        _initialPos = transform.position;
        _initialRot = transform.rotation;
        _rbody = GetComponent<Rigidbody>();
    }

    void Update() {
        if (transform.position.y < LOWER_BOUND) 
            ResetObject();
    }

    // Reset Transform
    void ResetObject() {
        _rbody.velocity = Vector3.zero;
        _rbody.MovePosition(_initialPos + Vector3.up * TOP_BOUND);
        _rbody.MoveRotation(_initialRot);
        _rbody.velocity = Vector3.zero;
    }
}
