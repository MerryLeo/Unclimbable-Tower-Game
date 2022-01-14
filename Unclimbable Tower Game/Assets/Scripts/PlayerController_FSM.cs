// Player controller that uses physics and a finite state machine to control an object

using System;
using System.Threading.Tasks;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlayerController_FSM : MonoBehaviour 
{
    [Header("Ground Settings")]
    [SerializeField]
    Transform groundCheck;
    [SerializeField]
    LayerMask groundMask;

    [Header("Movement Settings")]
    [SerializeField]
    float walkSpeed = 120f;
    [SerializeField]
    float runSpeed = 180f;
    [SerializeField]
    float midairSpeed = 10f;
    [SerializeField]
    float jumpForce = 450f;

    // States
    public PlayerBaseState CurrentState { get; private set; }
    public readonly PlayerIdleState IdleState = new PlayerIdleState();
    public readonly PlayerWalkingState WalkingState = new PlayerWalkingState();
    public readonly PlayerRunningState RunningState = new PlayerRunningState();
    public readonly PlayerJumpingState JumpingState = new PlayerJumpingState();
    public readonly PlayerFallingState FallingState = new PlayerFallingState();
    public event EventHandler<PlayerStateChangedEventArgs> StateChanged;


    // Propreties
    public bool Grounded => Physics.CheckSphere(groundCheck.position, groundDst, groundMask);
    public bool RigidBodyIsRising => rbody.velocity.y > 0.05;
    public bool RigidBodyIsFalling => rbody.velocity.y < 0.05;
    public Vector3 CurrentVelocity => rbody.velocity;
    public bool JumpingEnabled { get; private set; } = true;
    public float Drag => 0.65f;
    public float MidairDrag => 0.95f;
    public float HorizontalRunningFactor => 0.75f; // Decreases the horizontal movement speed of the player while he's running
    public float BackwardRunningFactor => 0.25f; // Decreases the backward movement speed of the player while he's running

    // public float RunningFOV => 80f;
    public Transform CameraTransform { get; private set; }
    public CameraController CameraControllerScript { get; private set; }

    // Private fields
    Rigidbody rbody;
    const float groundDst = 0.35f, jumpCooldown = 0.08f;

    // Awake is called when the script instance is being loaded
    void Awake() 
    {
        rbody = GetComponent<Rigidbody>();
        CameraTransform = GetComponentInChildren<Camera>().transform;
        CameraControllerScript = GetComponentInChildren<CameraController>();
        rbody.drag = 0;
        rbody.freezeRotation = true;
    }

    // Start is called before the first frame update
    void Start() 
    {
        TransitionToState(IdleState);
    }

    // The three update functions are on each state for the player movements
    void Update() 
    {
        CurrentState.Update(this);
    }

    void FixedUpdate() 
    {
        CurrentState.FixedUpdate(this);
    }

    void LateUpdate() 
    {
        CurrentState.LateUpdate(this);
    }

    public void TransitionToState(PlayerBaseState state) 
    {
        // Raise an event to notify other scripts such as the player audio controller
        EventHandler<PlayerStateChangedEventArgs> stateHandler = StateChanged;
        PlayerStateChangedEventArgs args = new PlayerStateChangedEventArgs();
        args.PreviousState = CurrentState;
        args.NewState = state;
        stateHandler?.Invoke(this, args);

        CurrentState = state;
        CurrentState.EnterState(this);
    }

    // Get a normalized vector corresponding to the direction the player wishes to move only on the x-axis and the z-axis
    public Vector3 GetMovementInputs() 
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = ((CameraTransform.forward - Vector3.up * CameraTransform.forward.y).normalized * verticalInput + CameraTransform.right.normalized * horizontalInput).normalized;
        return direction;
    }

    // Get a normalized vector corresponding to the direction the player wishes to move only on the x-axis and the z-axis, but the horizontal and forward movements are multiplied
    public Vector3 GetMovementInputs(float horizontalModifier, float verticalModifier) 
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = ((CameraTransform.forward - Vector3.up * CameraTransform.forward.y).normalized * verticalInput * verticalModifier + CameraTransform.right.normalized * horizontalInput * horizontalModifier).normalized;
        return direction;
    }

    // Apply a drag only on the x-axis and z-axis
    public void ApplyDrag(float drag)
    {
        rbody.velocity = new Vector3(rbody.velocity.x * drag, rbody.velocity.y, rbody.velocity.z * drag);
    }

    // Direction must be normalized before calling the method
    public void Walk(Vector3 direction) 
    { 
        rbody.AddForce(direction * walkSpeed, ForceMode.Acceleration);
    }

    // Direction must be normalized before calling the method 
    public void Run(Vector3 direction) 
    { 
        rbody.AddForce(direction * runSpeed, ForceMode.Acceleration);
    }

    public void MoveMidair(Vector3 direction) 
    {
        rbody.AddForce(direction * midairSpeed, ForceMode.Acceleration);
    }

    public void Jump() 
    {
        // JumpingCooldown();
        rbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public async void JumpingCooldown() 
    {
        float end = Time.time + jumpCooldown;
        JumpingEnabled = false;
        while (Time.time < end) 
        {
            await Task.Yield();
        }
        JumpingEnabled = true;
    }
}

public class PlayerStateChangedEventArgs : EventArgs
{
    public PlayerBaseState NewState;
    public PlayerBaseState PreviousState;
}