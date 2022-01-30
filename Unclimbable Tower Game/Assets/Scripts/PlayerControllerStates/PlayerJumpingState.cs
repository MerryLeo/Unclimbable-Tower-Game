using UnityEngine;
using System.Threading.Tasks;

public class PlayerJumpingState : PlayerBaseState {
    Vector3 _moveDirection;
    const float _fallingCooldown = 0.05f;
    bool _falling, _grounded, _checkIfFalling;

    public override void EnterState(PlayerController_FSM playerController) {
        // Initial Action
        playerController.Jump();
        FallingCooldown();
    }

    public override void Update(PlayerController_FSM playerController) {
        // Movement Direction
        _moveDirection = playerController.GetMovementInputs();

        // Boolean
        _falling = playerController.RigidBodyIsFalling && _checkIfFalling;
        _grounded = playerController.Grounded && !playerController.RigidBodyIsFalling && !playerController.RigidBodyIsRising;
    }
    public override void FixedUpdate(PlayerController_FSM playerController) {
        // Action
        playerController.MoveMidair(_moveDirection);
        playerController.ApplyDrag(playerController.MidairDrag);
    }

    public override void LateUpdate(PlayerController_FSM playerController) {
        if (_falling) // Transition to Falling State
            playerController.TransitionToState(playerController.FallingState);
        
        else if (_grounded) { // Transition to Idle State
            playerController.JumpingCooldown();
            playerController.TransitionToState(playerController.IdleState);
        }
    }

    // Can Transition to falling state after this cooldown
    async void FallingCooldown() {
        _checkIfFalling = false;
        float endTime = Time.time + _fallingCooldown;
        while (Time.time < endTime) {
            await Task.Yield();
        }
        _checkIfFalling = true;
    }
}
