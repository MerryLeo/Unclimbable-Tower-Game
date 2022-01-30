using UnityEngine;
public class PlayerWalkingState : PlayerBaseState {
    Vector3 _moveDirection;
    bool _grounded, _running, _walking, _jumping;
    const string _jumpInput = "Jump", _runInput = "Run";
    public override void EnterState(PlayerController_FSM playerController) { }

    public override void Update(PlayerController_FSM playerController) {
        // Movement Direction
        _moveDirection = playerController.GetMovementInputs();

        // Booleans
        _jumping = Input.GetButton(_jumpInput) && playerController.Grounded && playerController.JumpingEnabled;
        _walking = _moveDirection.magnitude > 0;
        _running = _walking && Input.GetButton(_runInput);
        _grounded = playerController.Grounded;
    }

    public override void FixedUpdate(PlayerController_FSM playerController) {
        // Actions
        playerController.Walk(_moveDirection);
        playerController.ApplyDrag(playerController.Drag);
    }

    public override void LateUpdate(PlayerController_FSM playerController) {
        // Transition to Falling State
        if (!_grounded)
            playerController.TransitionToState(playerController.FallingState);

        // Transition to Jumping State
        else if (_jumping)
            playerController.TransitionToState(playerController.JumpingState);

        // Transition to Running State
        else if (_running)
            playerController.TransitionToState(playerController.RunningState);

        // Transition to Idle State
        else if (!_walking)
            playerController.TransitionToState(playerController.IdleState);
    }
}
