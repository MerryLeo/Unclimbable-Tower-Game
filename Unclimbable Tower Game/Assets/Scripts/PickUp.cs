// A script that allows the player to pick up items
// This script needs to be attached to the player's camera

using UnityEngine;

[RequireComponent(typeof(CameraController))]
public class PickUp : MonoBehaviour 
{
    public PickupState CurrentState { get; private set; } = PickupState.IDLE;     
    const float maxPickupDst = 5f, maxHoldDst = 5.5f, minCameraObjectDst = 1.5f, force = 60f, throwForce = 275f, torque = 80f;
    const float pickupAngularDrag = 15f, pickupDrag = 10f, decelerationThreshold = 2.25f;
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
                if (CurrentState is PickupState.OBJECTINSIGHT && Input.GetButton("Fire1"))
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
        float distance = Vector3.Distance(objectData.ObjectRbody.position, transform.position);
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
    /*
    async void CalculateObjectSpeed()
    {
        float time = Time.time;
        speed = 0;
        while (speed == 0 && (CurrentState is PickupState.HOLDINGLIGHTOBJECT || CurrentState is PickupState.HOLDINGLIGHTOBJECT))
        {
            await Task.Yield();
        }
    }
    */

    /*
    [SerializeField]
    LayerMask unpickableLayer;
    public bool HoldingLightObject { get; private set; }
    public bool HoldingHeavyObject { get; private set; }
    public bool PickableObjectInSight { get; private set; }
    public bool RotatingTheObject { get; private set; }

    Rigidbody pickedObjectRb;
    Mesh pickedObjectMesh;
    const string pickupTag = "Pickup", holdingTag = "Holding";
    const float maxSpeedModifier = 400f, speedModifierRate = 0.002f, angularDragWhileHolding = 8f, angularDragWhileDragging = 10f, torqueModifier = 100f, throwForce = 275f, minHoldingDst = 1.75f;
    const float maxPickupDst = 5f, maxHoldDst = 5.5f, maxHoldWeight = 90f;
    float objectAngularDrag;
    AnimationCurve speedModifierCurve;
    float time, currentSpeedModifier;
    Vector3 holdDestination, objectCenterOfMass;
    Transform holdTrans;
    CameraController cameraController;

    // Start is called before the first frame update
    void Start() 
    {
        cameraController = GetComponent<CameraController>();
        RotatingTheObject = HoldingHeavyObject = HoldingLightObject = PickableObjectInSight = false;

        // Animation curve for the picked up object speed
        Keyframe[] keys = new Keyframe[2];
        keys[0] = new Keyframe(0,0,0,0);
        keys[1] = new Keyframe(1,1,0,0);
        speedModifierCurve = new AnimationCurve(keys);

        // Position where the player picked the object
        holdTrans = new GameObject("Hold Position").transform;
        holdTrans.SetParent(transform);
    }

    // Update is called once per frame
    void Update() 
    {
        // While holding any object
        if (HoldingHeavyObject || HoldingLightObject) 
        {

            // Time on the speed modifier curve
            time += speedModifierRate + Time.deltaTime;
            if (time > 1)
                time = 1;
        }

        // While holding a light object
        if (HoldingLightObject) 
        {

            // Throw the object
            if (Input.GetButton("Fire2")) 
            {
                ReleaseCurrentObject();
                pickedObjectRb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
            }

            // Release the object
            else if (!Input.GetButton("Fire1") || !CheckIfHoldableInSight())
                ReleaseCurrentObject();


            // Rotate the object
            else if (Input.GetButton("Rotate") && !RotatingTheObject) 
            {
                cameraController.enabled = false;

                // Change the object's center of mass
                pickedObjectRb.centerOfMass = holdTrans.localPosition;

                // Rotate
                RotatingTheObject = true;
                RotateCurrentObject();
            }

            // While rotating the object
            else if (Input.GetButton("Rotate") && RotatingTheObject) 
            {
                RotateCurrentObject();
            }

            // When rotating the object has stopped
            else if (!Input.GetButton("Rotate") && RotatingTheObject) 
            {
                cameraController.enabled = true;
                ResetCurrentObjectCenterOfMass();
                RotatingTheObject = false;
            }
        }
        // While holding a heavy object
        else if (HoldingHeavyObject) 
        {
            // Release the object
            if (!Input.GetButton("Fire1") || !CheckIfHoldableInSight()) // !Input.GetButton("Fire1") || !CheckIfHoldableInSight()
                ReleaseCurrentObject();
        }
        
        // While not holding an object
        else
        { 
            Vector3 hitPosition;
            PickableObjectInSight = CheckIfPickableInSight(out hitPosition);

            // Pick up the object found by the ray
            if (Input.GetButtonDown("Fire1") && PickableObjectInSight)
                PickupObject(ref pickedObjectRb, hitPosition);
        }
    }

    void FixedUpdate() 
    {

        // While holding any object
        if (HoldingLightObject || HoldingHeavyObject)
            holdDestination = transform.position + transform.forward * minHoldingDst;

        // While holding a light or a heavy object
        if (HoldingLightObject) 
            MoveCurrentObject(holdDestination);
        else if (HoldingHeavyObject) 
            DragCurrentObject(holdDestination);
    }

    // Return true if the player is looking at a pickable object
    bool CheckIfPickableInSight(out Vector3 hitPos) 
    {
        // Ray thrown from the camera
        RaycastHit hitInfo;
        Ray ray = new Ray(transform.position, transform.forward);

        // Two rays: one to check for an object and one to check if the player is trying to pickup and object that is below the character
        bool objectInSight = Physics.Raycast(ray, out hitInfo, maxPickupDst) && hitInfo.transform.gameObject.tag == pickupTag;
        bool objectIsCollidingWithPlane = Physics.Raycast(ray, maxPickupDst, unpickableLayer);
        if (!objectIsCollidingWithPlane && objectInSight) 
        {
            pickedObjectRb = hitInfo.transform.gameObject.GetComponent<Rigidbody>();
            hitPos = hitInfo.point;
            return true;
        } 
        else 
        {
            pickedObjectRb = null;
            hitPos = Vector3.zero;
            return false;
        }
    }

    // Return true if the player can still hold the object in his hand
    bool CheckIfHoldableInSight() 
    {
        RaycastHit hitInfo;
        Ray ray = new Ray(transform.position, pickedObjectRb.transform.position - transform.position);
        bool hitObject = Physics.Raycast(ray, out hitInfo, maxHoldDst);
        return hitObject && hitInfo.rigidbody.tag == holdingTag;
    }

    void ReleaseCurrentObject() 
    {
        pickedObjectRb.gameObject.layer = LayerMask.NameToLayer("Ground"); // Set the layer back
        pickedObjectRb.gameObject.tag = pickupTag;
        pickedObjectRb.angularDrag = objectAngularDrag;
        RotatingTheObject = false;
        cameraController.enabled = true;

        if (HoldingLightObject) 
        {
            pickedObjectRb.useGravity = true;
            HoldingLightObject = false;
        } 
        else if (HoldingHeavyObject) 
            HoldingHeavyObject = false;

        ResetCurrentObjectCenterOfMass();
    }

    void PickupObject(ref Rigidbody objectRb, Vector3 hitPosition) 
    {
        // Set the object's layer on a specific layer that the player cannot jump on
        objectRb.gameObject.layer = LayerMask.NameToLayer("Holding");
        objectRb.gameObject.tag = holdingTag;

        // Setup the object's rigidbody
        if (objectRb.mass <= maxHoldWeight) {
            HoldingLightObject = true;
            objectRb.useGravity = false;
            objectRb.angularDrag = angularDragWhileHolding;
        } else {
            HoldingHeavyObject = true;
            objectRb.angularDrag = angularDragWhileDragging;
        }
        objectCenterOfMass = objectRb.centerOfMass;
        objectAngularDrag = objectRb.drag;

        // Setup the position that the player will hold the object
        holdTrans.position = hitPosition;
        holdTrans.SetParent(objectRb.gameObject.transform);

        // Reset variables
        PickableObjectInSight = false;
        time = 0;
    }

    // Rotate the object
    void RotateCurrentObject() 
    {
        float horizontalInput = Input.GetAxis("Mouse X");
        float verticalInput = Input.GetAxis("Mouse Y");
        pickedObjectRb.AddTorque(torqueModifier * (Vector3.up * horizontalInput + transform.right * -verticalInput));
    }

    // Move smoothly the object that the player is currently holding
    void MoveCurrentObject(Vector3 dest) 
    {
        // Set the velocity of the object
        currentSpeedModifier = UtilityClass.Remap(speedModifierCurve.Evaluate(time), 0, 1, 0, maxSpeedModifier);
        Vector3 movement = (dest - holdTrans.position) * currentSpeedModifier;
        pickedObjectRb.velocity = movement / pickedObjectRb.mass;
    }

    // Move smoothly only on the z-axis and x-axis the object that the player is currently holding
    void DragCurrentObject(Vector3 dest) 
    {
        // Apply a force on the x-axis and z-axis on the object
        currentSpeedModifier = UtilityClass.Remap(speedModifierCurve.Evaluate(time), 0, 1, 0, maxSpeedModifier);
        Vector3 direction = dest - holdTrans.position;
        Vector3 movement = (new Vector3(direction.x, 0, direction.z)) * currentSpeedModifier * 0.02f;
        pickedObjectRb.AddForce(movement, ForceMode.Acceleration);
    }

    void ResetCurrentObjectCenterOfMass() 
    {
        pickedObjectRb.centerOfMass = objectCenterOfMass;
    }
    */
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