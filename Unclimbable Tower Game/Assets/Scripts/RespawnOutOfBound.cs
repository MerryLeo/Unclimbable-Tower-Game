// Respawn an object when it falls below lowerBound

using UnityEngine;

[RequireComponent(typeof(PickableObject))]
[RequireComponent(typeof(Rigidbody))]
public class RespawnOutOfBound : MonoBehaviour {
    Vector3 _initialPos;
    Quaternion _initialRot;
    Rigidbody _rbody;
    const float _lowerBound = -10f, _topBound = 10f;
    void Start() {
        _initialPos = transform.position;
        _initialRot = transform.rotation;
        _rbody = GetComponent<Rigidbody>();
    }

    void Update() {
        if (transform.position.y < _lowerBound) 
            ResetObject();
    }

    // Reset Transform
    void ResetObject() {
        _rbody.velocity = Vector3.zero;
        _rbody.MovePosition(_initialPos + Vector3.up * _topBound);
        _rbody.MoveRotation(_initialRot);
        _rbody.velocity = Vector3.zero;
    }
}
