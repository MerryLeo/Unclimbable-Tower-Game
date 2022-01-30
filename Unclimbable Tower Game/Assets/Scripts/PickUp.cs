// A script that allows the player to pick up items
// This script needs to be attached to the player's camera

using UnityEngine;
using System;

[RequireComponent(typeof(CameraController))]
public class PickUp : MonoBehaviour {
    public PickupState CurrentState { get; private set; } = PickupState.IDLE;
    public event EventHandler<PickupStateEventArgs> PickupStateChanged;
    const float _maxPickupDst = 5f, _maxHoldDst = 5.5f, _minCameraObjectDst = 1.5f, _force = 60f, _throwForce = 275f, _torque = 80f;
    const float _pickupAngularDrag = 15f, _pickupDrag = 10f;
    const string _mouseXAxisName = "Mouse X", _mouseYAxisName = "Mouse Y";
    const int _weightThreshold = 80; // Object heavier than weightThreshold will be considered heavy
    LayerMask _holdingLayer;
    Transform _pickedTrans, _rotationTrans;
    AnimationCurve _pickupSpeedCurve;
    float _currentForce, _time;
    ObjectData _objectData;
    CameraController _camController;
    void Start() {
        // Location where the player picked the object
        _pickedTrans = new GameObject("PickedLocation").transform;
        _pickedTrans.SetParent(transform);

        // Empty Trans used for rotation computation solely
        _rotationTrans = new GameObject().transform;

        // Layers to prohibit the player from jumping on the object in his hand
        _holdingLayer = LayerMask.NameToLayer("Holding");

        // Camera Controller Script used to disable camera movement
        _camController = GetComponent<CameraController>();

        // Curve for the speed of the object when it is picked
        Keyframe[] keys = new Keyframe[2];
        keys[0] = new Keyframe(0,0,0,0);
        keys[1] = new Keyframe(1,1,0,0);
        _pickupSpeedCurve = new AnimationCurve(keys);
    }

    void Update() {
        // Idle
        if (CurrentState is PickupState.IDLE || CurrentState is PickupState.OBJECTINSIGHT) {
            RaycastHit hitInfo;
            Ray ray = new Ray(transform.position, transform.forward);
            bool pickableObjectInSight = false;
            if (Physics.Raycast(ray, out hitInfo, _maxPickupDst)) { // Throw Ray to check for object
                PickableObject objectScript = hitInfo.transform.GetComponent<PickableObject>();
                pickableObjectInSight = objectScript?.CurrentState is ObjectState.PICKABLE;
                if (CurrentState is PickupState.IDLE && pickableObjectInSight) // Change State to OBJECTINSIGHT
                    SetState(PickupState.OBJECTINSIGHT);
                
                if (CurrentState is PickupState.OBJECTINSIGHT && Input.GetButtonDown("Fire1")) // Change State to HOLDINGLIGHTOBJECT or HOLDINGHEAVYOBJECT
                    Pickup(objectScript, hitInfo);
            }
            else if (CurrentState is PickupState.OBJECTINSIGHT && !pickableObjectInSight) { // Change State to IDLE
                SetState(PickupState.IDLE);
            }
        }

        else if (CurrentState is PickupState.HOLDINGHEAVYOBJECT || CurrentState is PickupState.HOLDINGLIGHTOBJECT) { // Holding Object
            // Throw
            if (CurrentState is PickupState.HOLDINGLIGHTOBJECT && Input.GetButton("Fire2"))
                Throw();

            // Release
            if (!Input.GetButton("Fire1") || Vector3.Distance(transform.position, _objectData.ObjectRbody.transform.position) > _maxHoldDst)
                Release();

            else if (CurrentState is PickupState.HOLDINGLIGHTOBJECT && Input.GetButton("Rotate")) {
                SetState(PickupState.ROTATINGLIGHTOBJECT);
                _camController.enabled = false;
            }
        }
        
        if (CurrentState is PickupState.ROTATINGLIGHTOBJECT) { // Rotating Object
            if (Input.GetButton("Fire2"))
                Throw();
            
            else if (!Input.GetButton("Fire1") || Vector3.Distance(transform.position, _objectData.ObjectRbody.transform.position) > _maxHoldDst)
                Release();

            else if (!Input.GetButton("Rotate"))
                SetState(PickupState.HOLDINGLIGHTOBJECT);
                // CurrentState = PickupState.HOLDINGLIGHTOBJECT;

            if (!(CurrentState is PickupState.ROTATINGLIGHTOBJECT)) {
                _camController.enabled = true;
                _pickedTrans.forward = transform.forward;
            }
        }
    }

    void FixedUpdate() {
        // Move object
        if (CurrentState is PickupState.HOLDINGLIGHTOBJECT || CurrentState is PickupState.HOLDINGHEAVYOBJECT || CurrentState is PickupState.ROTATINGLIGHTOBJECT) {
            _currentForce = UtilityClass.Remap(_pickupSpeedCurve.Evaluate(Time.time - _time), 0, 1, 0, _force); // Force is a value between 0 and maxForce
            Vector3 destination = transform.position + transform.forward * _minCameraObjectDst;
            if (CurrentState is PickupState.HOLDINGLIGHTOBJECT) {
                Move(destination, _currentForce);
                AlignRotation(transform.forward);
            }
            else if (CurrentState is PickupState.ROTATINGLIGHTOBJECT) {
                Move(destination, _currentForce);
                Rotate();
            }
            else
                Drag(destination, _currentForce);
            
        }
    }

    // Change Pickup State and Invoke PickupStateChanged
    void SetState(PickupState newState) {
        EventHandler<PickupStateEventArgs> handler = PickupStateChanged;
        PickupStateEventArgs args = new PickupStateEventArgs();
        args.NewState = newState;
        handler?.Invoke(this, args);
        CurrentState = newState;
    }

    #region Pickup Actions

    // Release Object
    void Release() {
        // Reset the rigidbody and Change State
        _objectData.ResetRbody();
        SetState(PickupState.IDLE);
        _objectData.ObjectScript.ChangeState(ObjectState.PICKABLE);
    }

    // Pickup Object
    void Pickup(PickableObject pickup, RaycastHit hitInfo) {
        // Set some variables to reset the rigidbody when released
        _objectData = new ObjectData(pickup);

        // Modify empty transform for positioning and rotation
        _pickedTrans.transform.position = hitInfo.point;
        _pickedTrans.transform.rotation = transform.rotation;
        _pickedTrans.SetParent(_objectData.ObjectRbody.transform);

        // Time variable used for the force
        _time = Time.time;

        // Change state
        PickupState newState = (_objectData.ObjectRbody.mass > _weightThreshold) ? PickupState.HOLDINGHEAVYOBJECT : PickupState.HOLDINGLIGHTOBJECT;
        SetState(newState);
        this._objectData.ObjectScript.ChangeState(ObjectState.HELD);

        // Modify Rigidbody
        _objectData.UpdateRbody(_pickupDrag, _pickupAngularDrag, true, _holdingLayer);
    }

    // Move Object to Position with Force
    void Move(Vector3 position, float force) {
        Vector3 movement = (position - _pickedTrans.transform.position) * (force / _objectData.ObjectRbody.mass);
        float strength = 0.8f / Vector3.Distance(_objectData.ObjectRbody.position, transform.position);
        Vector3 repulsion = strength * (_objectData.ObjectRbody.position - transform.position);
        movement += repulsion;
        _objectData.ObjectRbody.AddForce(movement, ForceMode.VelocityChange);
    }

    // Drag Object along the x-axis/z-axis to Position with Force
    void Drag(Vector3 position, float force) {
        Vector3 movement = (position - _pickedTrans.transform.position) * (force / _objectData.ObjectRbody.mass);
        movement -= Vector3.up * movement.y;
        _objectData.ObjectRbody.AddForce(movement, ForceMode.VelocityChange);
    }

    // Throw Object forward
    void Throw() {
        Release();
        _objectData.ObjectRbody.AddForce(transform.forward * _throwForce, ForceMode.Impulse);
        _objectData.ObjectScript.ChangeState(ObjectState.THROWN);
    }

    // Rotate the object using mouse inputs
    void Rotate() {
        float horizontalInput = Input.GetAxis(_mouseXAxisName);
        float verticalInput = Input.GetAxis(_mouseYAxisName);
        Vector3 torqueDir = -transform.right * verticalInput + Vector3.up * horizontalInput; 
        _objectData.ObjectRbody.AddTorque(torqueDir * _torque, ForceMode.Acceleration);
    }

    #endregion

    // Align the pickedTrans forward with direction
    void AlignRotation(Vector3 direction) {
        Vector3 rot1 = Vector3.Cross(_pickedTrans.transform.forward - Vector3.up * _pickedTrans.transform.forward.y, direction - Vector3.up * direction.y);
        Vector3 rot2 = Vector3.Cross(_pickedTrans.transform.forward,direction);
        rot2 -= Vector3.up * rot2.y;
        _rotationTrans.transform.forward = direction;
        _rotationTrans.RotateAround(_rotationTrans.transform.position, Vector3.Cross(direction, Vector3.up), 90);
        Vector3 rot3 = Vector3.Cross(_pickedTrans.transform.up, _rotationTrans.forward);
        Vector3 torqueDir = rot1 + rot2 + rot3;
        _objectData.ObjectRbody.AddTorque(torqueDir * _torque, ForceMode.Acceleration);
    }

    // Class used to save and update an object's rigidbody variables
    class ObjectData {
        public PickableObject ObjectScript { get; private set; }
        public Rigidbody ObjectRbody { get; private set; }
        LayerMask _defaultLayer;
        float _drag, _angularDrag;
        bool _gravity;
        
        // Constructor to Save Data
        public ObjectData(PickableObject objectScript) {
            this.ObjectScript = objectScript;

            // Fetch Rigidbody Data
            this.ObjectRbody = objectScript.Rbody;
            this._drag = ObjectRbody.drag;
            this._angularDrag = ObjectRbody.angularDrag;
            this._gravity = ObjectRbody.useGravity;

            this._defaultLayer = objectScript.gameObject.layer;
        }

        // Modify Object Rigidbody Propreties
        public void UpdateRbody(float drag, float angularDrag, bool useGravity, LayerMask mask) {
            ObjectRbody.drag = drag;
            ObjectRbody.angularDrag = angularDrag;
            ObjectRbody.useGravity = useGravity;
            ObjectRbody.gameObject.layer = mask;
        }

        // Reset Rigidbody Propreties
        public void ResetRbody() {
            ObjectRbody.drag = this._drag;
            ObjectRbody.angularDrag = this._angularDrag;
            ObjectRbody.useGravity = this._gravity;
            ObjectRbody.gameObject.layer = this._defaultLayer;
        }
    }
}

public class PickupStateEventArgs : EventArgs {
    public PickupState NewState;
}

public enum PickupState {
        HOLDINGLIGHTOBJECT,
        HOLDINGHEAVYOBJECT,
        ROTATINGLIGHTOBJECT,
        OBJECTINSIGHT,
        IDLE,
        INACTIVE
    }