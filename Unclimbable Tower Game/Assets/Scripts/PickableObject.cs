


using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PickableObject : MonoBehaviour
{
    public ObjectState CurrentState { get; private set; }
    public Rigidbody Rbody { get; private set; }
    LayerMask playerMask;
    GameObject playerObj;
    const float distThreshold = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        Rbody = GetComponent<Rigidbody>();
        ChangeState(ObjectState.PICKABLE);
        GetComponent<Rigidbody>().detectCollisions = true;
        playerMask = LayerMask.NameToLayer("Player");
        playerObj = GameObject.FindObjectOfType<PlayerController_FSM>().gameObject;
    }

    void Update() 
    {
        if (CurrentState is ObjectState.HELD && Vector3.Distance(transform.position, playerObj.transform.position) < distThreshold)
        {
            Debug.Log("In Range");
        }
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
