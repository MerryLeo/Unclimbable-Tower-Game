// Player controller that uses physics and a finite state machine to control an object

using System;
using System.Threading.Tasks;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlayerController : MonoBehaviour 
{
    const float _groundDist = 0.35f, _jumpCooldown = 0.08f;
    const string _horizontalAxisName = "Horizontal", _verticalAxisName = "Vertical";

    [Header("Ground Settings")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;

    [Header("Movement Settings")]
    
    [SerializeField] float walkSpeed = 120f, runSpeed = 180f, midairSpeed = 10f, jumpForce = 450f;
    
    public PlayerBaseState CurrentState { get; private set; }
    public readonly PlayerIdleState IdleState = new PlayerIdleState();
    public readonly PlayerWalkingState WalkingState = new PlayerWalkingState();
    public readonly PlayerRunningState RunningState = new PlayerRunningState();
    public readonly PlayerJumpingState JumpingState = new PlayerJumpingState();
    public readonly PlayerFallingState FallingState = new PlayerFallingState();
    public event EventHandler<PlayerStateChangedEventArgs> StateChanged;
    
    public bool Grounded => Physics.CheckSphere(groundCheck.position, _groundDist, groundMask);
    public bool RigidBodyIsRising => _rbody.velocity.y > 0.05;
    public bool RigidBodyIsFalling => _rbody.velocity.y < 0.05;
    public Vector3 CurrentVelocity => _rbody.velocity;
    public bool JumpingEnabled { get; private set; } = true;
    public float Drag => 0.65f;
    public float MidairDrag => 0.95f;
    public float HorizontalRunningFactor => 0.75f; // Decreases the horizontal movement speed of the player while he's running
    public float BackwardRunningFactor => 0.25f; // Decreases the backward movement speed of the player while he's running
    public Transform CameraTransform { get; private set; }

    Rigidbody _rbody;

    void Awake() 
    {
        _rbody = GetComponent<Rigidbody>();
        CameraTransform = GetComponentInChildren<Camera>().transform;
        _rbody.drag = 0;
        _rbody.freezeRotation = true;
    }

    void Start() 
    {
        TransitionToState(IdleState);
    }

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

    // Raise an event to notify other scripts such as the player audio controller
    public void TransitionToState(PlayerBaseState state) 
    {
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
        float horizontalInput = Input.GetAxis(_horizontalAxisName);
        float verticalInput = Input.GetAxis(_verticalAxisName);
        Vector3 direction = ((CameraTransform.forward - Vector3.up * CameraTransform.forward.y).normalized * verticalInput + CameraTransform.right.normalized * horizontalInput).normalized;
        return direction;
    }

    // Get a normalized vector corresponding to the direction the player wishes to move only on the x-axis and the z-axis, but the horizontal and forward movements are multiplied
    public Vector3 GetMovementInputs(float horizontalModifier, float verticalModifier) 
    {
        float horizontalInput = Input.GetAxis(_horizontalAxisName);
        float verticalInput = Input.GetAxis(_verticalAxisName);
        Vector3 direction = ((CameraTransform.forward - Vector3.up * CameraTransform.forward.y).normalized * verticalInput * verticalModifier + CameraTransform.right.normalized * horizontalInput * horizontalModifier).normalized;
        return direction;
    }

    // Apply a drag only on the x-axis and z-axis
    public void ApplyDrag(float drag) 
    {
        _rbody.velocity = new Vector3(_rbody.velocity.x * drag, _rbody.velocity.y, _rbody.velocity.z * drag);
    }

    // Direction must be normalized before calling the method
    public void Walk(Vector3 direction) 
    { 
        _rbody.AddForce(direction * walkSpeed, ForceMode.Acceleration);
    }

    // Direction must be normalized before calling the method 
    public void Run(Vector3 direction) 
    { 
        _rbody.AddForce(direction * runSpeed, ForceMode.Acceleration);
    }

    public void MoveMidair(Vector3 direction) 
    {
        _rbody.AddForce(direction * midairSpeed, ForceMode.Acceleration);
    }

    public void Jump() 
    {
        // JumpingCooldown();
        _rbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public async void JumpingCooldown() 
    {
        float end = Time.time + _jumpCooldown;
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