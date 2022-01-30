using UnityEngine;
public class PlayerRunningState : PlayerBaseState
{
    Vector3 _moveDirection;
    bool _grounded, _jumping, _walking, _running;
    const string _verticalAxisName = "Vertical", _horizontalAxisName = "Horizontal";
    const string _runInput = "Run", _jumpInput = "Jump";
    public override void EnterState(PlayerController_FSM playerController) { }
    public override void Update(PlayerController_FSM playerController) {
        // Movement Direction
        float verticalAxis = Input.GetAxis(_verticalAxisName);
        float horizontalAxis = Input.GetAxis(_horizontalAxisName);
        _moveDirection = playerController.GetMovementInputs(playerController.HorizontalRunningFactor, 1);

        // Booleans
        _grounded = playerController.Grounded;
        _walking = _moveDirection.magnitude > 0 && !Input.GetButton(_runInput);
        _running = _moveDirection.magnitude > 0 && Input.GetButton(_runInput);
        _jumping = Input.GetButton(_jumpInput) && playerController.Grounded && playerController.JumpingEnabled;
    }

    public override void FixedUpdate(PlayerController_FSM playerController) {
        // Actions
        playerController.Run(_moveDirection);
        playerController.ApplyDrag(playerController.Drag);
    }

    public override void LateUpdate(PlayerController_FSM playerController) {
        // Transition to Falling State
        if (!_grounded) 
            playerController.TransitionToState(playerController.FallingState);

        // Transition to Jumping State
        else if (_jumping) 
            playerController.TransitionToState(playerController.JumpingState);
            
        // Transition to Walking State
        else if (_walking)
            playerController.TransitionToState(playerController.WalkingState);
        
        // Transition to Idle State
        else if (!_running)
            playerController.TransitionToState(playerController.IdleState);
    }
}
