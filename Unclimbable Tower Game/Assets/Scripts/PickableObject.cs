

using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PickableObject : MonoBehaviour
{
    public ObjectState CurrentState { get; private set; }
    public Rigidbody Rbody { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Rbody = GetComponent<Rigidbody>();
        ChangeState(ObjectState.PICKABLE);
        GetComponent<Rigidbody>().detectCollisions = true;
    }

    public void ChangeState(ObjectState state)
    {
        CurrentState = state;
    }

    void OnCollisionEnter(Collision other) 
    {
        if (CurrentState is ObjectState.THROWN)
            ChangeState(ObjectState.PICKABLE);
    }

    void OnCollisionStay(Collision other) 
    {
        if (CurrentState is ObjectState.THROWN)
            ChangeState(ObjectState.PICKABLE);
    }
}

public enum ObjectState
{
    PICKABLE,
    HELD,
    THROWN,
    DISABLED
}
