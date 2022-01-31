using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PickableObject : MonoBehaviour {
    public ObjectState CurrentState { get; private set; }
    public Rigidbody Rbody { get; private set; }
    LayerMask _playerMask;
    GameObject _playerObj;
    const float _distThreshold = 0.5f;

    void Start() {
        Rbody = GetComponent<Rigidbody>();
        ChangeState(ObjectState.PICKABLE);
        GetComponent<Rigidbody>().detectCollisions = true;
        _playerMask = LayerMask.NameToLayer("Player");
        _playerObj = GameObject.FindObjectOfType<PlayerController_FSM>().gameObject;
    }

    // NEED MODIFICATION!!!!
    void Update() {
        /*
        if (CurrentState is ObjectState.HELD && Vector3.Distance(transform.position, _playerObj.transform.position) < _distThreshold) {
            Debug.Log("In Range");
        }
        */
    }

    public void ChangeState(ObjectState state) {
        CurrentState = state;
    }

    void OnCollisionEnter(Collision other)  {
        if (CurrentState is ObjectState.THROWN)
            ChangeState(ObjectState.PICKABLE);
    }

    void OnCollisionStay(Collision other) {
        if (CurrentState is ObjectState.THROWN)
            ChangeState(ObjectState.PICKABLE);
    }
}

public enum ObjectState {
    PICKABLE,
    HELD,
    THROWN,
    DISABLED
}
