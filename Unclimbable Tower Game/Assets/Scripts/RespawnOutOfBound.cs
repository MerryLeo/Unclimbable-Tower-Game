// Respawn an object when it falls below lowerBound

using UnityEngine;

[RequireComponent(typeof(PickableObject))]
[RequireComponent(typeof(Rigidbody))]
public class RespawnOutOfBound : MonoBehaviour 
{
    Vector3 initialPos;
    Quaternion initialRot;
    const float lowerBound = -10f, topBound = 10f;
    Rigidbody rbody;

    // Start is called before the first frame update
    void Start() 
    {
        initialPos = transform.position;
        initialRot = transform.rotation;
        rbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() 
    {
        if (transform.position.y < lowerBound) 
            ResetObject();
    }

    void ResetObject() 
    {
        rbody.velocity = Vector3.zero;
        rbody.MovePosition(initialPos + Vector3.up * topBound);
        rbody.MoveRotation(initialRot);
        rbody.velocity = Vector3.zero;
    }
}
