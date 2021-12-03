// A script that allows the player to pick up items
// This script needs to be attached to the player's camera

using UnityEngine;


[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(CameraController))]
public class PickUp : MonoBehaviour 
{
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
    
}