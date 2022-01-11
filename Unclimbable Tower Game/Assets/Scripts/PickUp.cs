// A script that allows the player to pick up items
// This script needs to be attached to the player's camera

using UnityEngine;

[RequireComponent(typeof(CameraController))]
public class PickUp : MonoBehaviour 
{
    public PickupState CurrentState { get; private set; } = PickupState.IDLE;     
    const float maxPickupDst = 5f, maxHoldDst = 5.5f, minCameraObjectDst = 1.5f, force = 60f, throwForce = 275f, torque = 80f;
    const float pickupAngularDrag = 15f, pickupDrag = 10f;
    Vector2 decelerationDist = new Vector2(2.25f, 1.5f);
    LayerMask holdingLayer;
    Transform pickedTrans, rotationTrans;
    AnimationCurve pickupSpeedCurve;
    float currentForce, time;
    ObjectData objectData;
    CameraController camController;
    const int weightThreshold = 80; // Object heavier than weightThreshold will be considered heavy
    void Start()
    {
        // Location where the player picked the object
        pickedTrans = new GameObject("PickedLocation").transform;
        pickedTrans.SetParent(transform);

        // Empty Trans used for rotation computation solely
        rotationTrans = new GameObject().transform;

        // Layers to prohibit the player from jumping on the object in his hand
        holdingLayer = LayerMask.NameToLayer("Holding");

        // Camera Controller Script used to disable camera movement
        camController = GetComponent<CameraController>();

        // Curve for the speed of the object when it is picked
        Keyframe[] keys = new Keyframe[2];
        keys[0] = new Keyframe(0,0,0,0);
        keys[1] = new Keyframe(1,1,0,0);
        pickupSpeedCurve = new AnimationCurve(keys);
    }

    void Update()
    {
        // Idle
        if (CurrentState is PickupState.IDLE || CurrentState is PickupState.OBJECTINSIGHT)
        {
            // Ray from camera
            RaycastHit hitInfo;
            Ray ray = new Ray(transform.position, transform.forward);
            bool rayHit = Physics.Raycast(ray, out hitInfo, maxPickupDst);
            PickableObject objectScript;
            if (rayHit)
            {
                objectScript = hitInfo.transform.GetComponent<PickableObject>();
                bool pickableObjectInSight = rayHit && objectScript?.CurrentState is ObjectState.PICKABLE;
                CurrentState = pickableObjectInSight ? PickupState.OBJECTINSIGHT : PickupState.IDLE;

                // Pickup
                if (CurrentState is PickupState.OBJECTINSIGHT && Input.GetButtonDown("Fire1"))
                    Pickup(objectScript, hitInfo);
            }
        }

        // Holding something
        else if (CurrentState is PickupState.HOLDINGHEAVYOBJECT || CurrentState is PickupState.HOLDINGLIGHTOBJECT)
        {
            // Throw
            if (CurrentState is PickupState.HOLDINGLIGHTOBJECT && Input.GetButton("Fire2"))
                Throw();

            // Release
            if (!Input.GetButton("Fire1") || Vector3.Distance(transform.position, objectData.ObjectRbody.transform.position) > maxHoldDst)
                Release();

            else if (CurrentState is PickupState.HOLDINGLIGHTOBJECT && Input.GetButton("Rotate"))
            {
                CurrentState = PickupState.ROTATINGLIGHTOBJECT;
                camController.enabled = false;
            }
        }

        // Rotating object
        if (CurrentState is PickupState.ROTATINGLIGHTOBJECT)
        {
            if (Input.GetButton("Fire2"))
                Throw();
            
            else if (!Input.GetButton("Fire1") || Vector3.Distance(transform.position, objectData.ObjectRbody.transform.position) > maxHoldDst)
                Release();

            else if (!Input.GetButton("Rotate"))
                CurrentState = PickupState.HOLDINGLIGHTOBJECT;

            if (!(CurrentState is PickupState.ROTATINGLIGHTOBJECT))
            {
                camController.enabled = true;
                pickedTrans.forward = transform.forward;
            }
        }
    }

    void FixedUpdate() 
    {
        // Move object
        if (CurrentState is PickupState.HOLDINGLIGHTOBJECT || CurrentState is PickupState.HOLDINGHEAVYOBJECT || CurrentState is PickupState.ROTATINGLIGHTOBJECT)
        {
            currentForce = UtilityClass.Remap(pickupSpeedCurve.Evaluate(Time.time - time), 0, 1, 0, force); // Force is a value between 0 and maxForce
            Vector3 destination = transform.position + transform.forward * minCameraObjectDst;
            if (CurrentState is PickupState.HOLDINGLIGHTOBJECT)
            {
                Move(destination, currentForce);
                AlignRotation(transform.forward);
            }
            else if (CurrentState is PickupState.ROTATINGLIGHTOBJECT)
            {
                Move(destination, currentForce);
                Rotate();
            }
            else
            {
                Drag(destination, currentForce);
            }
        }
    }

    // Release the current object
    void Release()
    {
        // Reset the rigidbody and Change State
        objectData.ResetRigidbody();
        CurrentState = PickupState.IDLE;
        objectData.ObjectScript.ChangeState(ObjectState.PICKABLE);
    }

    // Pickup
    void Pickup(PickableObject pickup, RaycastHit hitInfo)
    {
        // Set some variables to reset the rigidbody when released
        objectData = new ObjectData(pickup);

        // Modify empty transform for positioning and rotation
        pickedTrans.transform.position = hitInfo.point;
        pickedTrans.transform.rotation = transform.rotation;
        pickedTrans.SetParent(objectData.ObjectRbody.transform);

        // Time variable used for the force
        time = Time.time;

        // Change state
        this.CurrentState = (objectData.ObjectRbody.mass > weightThreshold) ? PickupState.HOLDINGHEAVYOBJECT : PickupState.HOLDINGLIGHTOBJECT;
        this.objectData.ObjectScript.ChangeState(ObjectState.HELD);

        // Modify Rigidbody
        objectData.UpdateRbody(pickupDrag, pickupAngularDrag, true, holdingLayer);
    }

    // Move the object to position using forces
    void Move(Vector3 position, float force)
    {
        Vector3 movement = (position - pickedTrans.transform.position) * (force / objectData.ObjectRbody.mass);

        

        float strength = 0.8f / Vector3.Distance(objectData.ObjectRbody.position, transform.position);
        Vector3 repulsion = strength * (objectData.ObjectRbody.position - transform.position);
        movement += repulsion;
        // Decelerate the object if it is too close to the player
        
        /*
        if (distance < decelerationThreshold)
        {
            float percentage = Mathf.InverseLerp(decelerationThreshold, 1.5f, Mathf.Clamp(distance, 1.5f, decelerationThreshold));

            Vector3 relativePos = transform.position - objectData.ObjectRbody.position;
            Vector3 deceleration = Vector3.zero;
            if (Mathf.Sign(relativePos.x) == Mathf.Sign(movement.x))
            {
                deceleration += Vector3.right * Mathf.Lerp(0, movement.x, percentage);
            }

            if (Mathf.Sign(relativePos.y) == Mathf.Sign(movement.y))
            {
                deceleration += Vector3.up * Mathf.Lerp(0, movement.y, percentage);
            }

            if (Mathf.Sign(relativePos.z) == Mathf.Sign(movement.z))
            {
                deceleration += Vector3.forward * Mathf.Lerp(0, movement.z, percentage);
            }
            movement -= deceleration;
        }
        */
        objectData.ObjectRbody.AddForce(movement, ForceMode.VelocityChange);
    }

    // Drag the object along the x-axis/z-axis using forces
    void Drag(Vector3 position, float force)
    {
        Vector3 movement = (position - pickedTrans.transform.position) * (force / objectData.ObjectRbody.mass);
        movement -= Vector3.up * movement.y;
        objectData.ObjectRbody.AddForce(movement, ForceMode.VelocityChange);
    }

    void Throw()
    {
        Release();
        objectData.ObjectRbody.AddForce(transform.forward * throwForce, ForceMode.Impulse);
        objectData.ObjectScript.ChangeState(ObjectState.THROWN);
    }

    // Rotate the object using mouse inputs
    void Rotate()
    {
        float horizontalInput = Input.GetAxis("Mouse X");
        float verticalInput = Input.GetAxis("Mouse Y");
        Vector3 torqueDir = -transform.right * verticalInput + Vector3.up * horizontalInput; 
        objectData.ObjectRbody.AddTorque(torqueDir * torque, ForceMode.Acceleration);
    }

    // Align the pickedTrans forward with forward
    void AlignRotation(Vector3 forward)
    {
        Vector3 rot1 = Vector3.Cross(pickedTrans.transform.forward - Vector3.up * pickedTrans.transform.forward.y, forward - Vector3.up * forward.y);
        Vector3 rot2 = Vector3.Cross(pickedTrans.transform.forward,forward);
        rot2 -= Vector3.up * rot2.y;
        rotationTrans.transform.forward = forward;
        rotationTrans.RotateAround(rotationTrans.transform.position, Vector3.Cross(forward, Vector3.up), 90);
        Vector3 rot3 = Vector3.Cross(pickedTrans.transform.up, rotationTrans.forward);
        Vector3 torqueDir = rot1 + rot2 + rot3;
        objectData.ObjectRbody.AddTorque(torqueDir * torque, ForceMode.Acceleration);
    }

    // Class used to save and update an object rigidbody's variables
    class ObjectData
    {
        public PickableObject ObjectScript { get; private set; }
        public Rigidbody ObjectRbody { get; private set; }
        LayerMask defaultLayer;
        float drag, angularDrag;
        bool gravity;
        
        // Constructor to Save Data
        public ObjectData(PickableObject objectScript)
        {
            this.ObjectScript = objectScript;

            // Fetch Rigidbody Data
            this.ObjectRbody = objectScript.Rbody;
            this.drag = ObjectRbody.drag;
            this.angularDrag = ObjectRbody.angularDrag;
            this.gravity = ObjectRbody.useGravity;

            this.defaultLayer = objectScript.gameObject.layer;
        }

        // Modify Object Rigidbody Propreties
        public void UpdateRbody(float drag, float angularDrag, bool useGravity, LayerMask mask)
        {
            ObjectRbody.drag = drag;
            ObjectRbody.angularDrag = angularDrag;
            ObjectRbody.useGravity = useGravity;
            ObjectRbody.gameObject.layer = mask;
        }

        // Reset Rigidbody Propreties
        public void ResetRigidbody()
        {
            ObjectRbody.drag = this.drag;
            ObjectRbody.angularDrag = this.angularDrag;
            ObjectRbody.useGravity = this.gravity;
            ObjectRbody.gameObject.layer = this.defaultLayer;
        }
    }
}

public enum PickupState
{
    HOLDINGLIGHTOBJECT,
    HOLDINGHEAVYOBJECT,
    ROTATINGLIGHTOBJECT,
    OBJECTINSIGHT,
    IDLE,
    INACTIVE
}