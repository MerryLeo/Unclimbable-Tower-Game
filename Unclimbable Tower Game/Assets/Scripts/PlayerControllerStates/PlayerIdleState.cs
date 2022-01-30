using UnityEngine;
public class PlayerIdleState : PlayerBaseState {
    bool _grounded, _walking, _running, _jumping;
    const string _horizontalAxisName = "Horizontal", _verticalAxisName = "Vertical";
    const string _runInput = "Run", _jumpInput = "Jump";
    public override void EnterState(PlayerController_FSM playerController) { }
    public override void Update(PlayerController_FSM playerController) {
        // Booleans
        _grounded = playerController.Grounded;
        _walking = Input.GetAxis(_horizontalAxisName) != 0 || Input.GetAxis(_verticalAxisName) != 0;
        _running = _walking && Input.GetButton(_runInput);
        _jumping = Input.GetButton(_jumpInput) && playerController.Grounded && playerController.JumpingEnabled;
    }
    public override void FixedUpdate(PlayerController_FSM playerController) { 
        // Action
        if (playerController.CurrentVelocity.magnitude > 0)
            playerController.ApplyDrag(playerController.Drag);
    }

    public override void LateUpdate(PlayerController_FSM playerController) {
        // Transition to Falling State
        if (!_grounded)
            playerController.TransitionToState(playerController.FallingState);
        // Transition to Jumping State
        if (_jumping) 
            playerController.TransitionToState(playerController.JumpingState);
        
        // Transition to Running State
        else if (_running)
            playerController.TransitionToState(playerController.RunningState);
        
        // Transition to Walking State
        else if (_walking) 
            playerController.TransitionToState(playerController.WalkingState); 
    }
}
