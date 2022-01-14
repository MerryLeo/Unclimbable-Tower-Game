using UnityEngine;

public class PlayerJumpingState : PlayerBaseState
{
    Vector3 moveDirection;
    const float cooldown = 0.05f;
    bool falling, grounded, checkIfFalling;
    float time;
    public override void EnterState(PlayerController_FSM playerController)
    {
        // Initial Action
        playerController.Jump();

        // Time Stamp
        checkIfFalling = false;
        time = Time.time + cooldown;
    }

    public override void Update(PlayerController_FSM playerController)
    {
        // Movement Direction
        moveDirection = playerController.GetMovementInputs();

        // Cooldown
        if (Time.time >= time)
            checkIfFalling = true;

        // Boolean
        falling = playerController.RigidBodyIsFalling && checkIfFalling;
        grounded = playerController.Grounded && !playerController.RigidBodyIsFalling && !playerController.RigidBodyIsRising;
    }
    public override void FixedUpdate(PlayerController_FSM playerController)
    {
        // Action
        playerController.MoveMidair(moveDirection);
        playerController.ApplyDrag(playerController.MidairDrag);
    }

    public override void LateUpdate(PlayerController_FSM playerController)
    {
        // Transition to Falling State
        if (falling)
        {
            playerController.TransitionToState(playerController.FallingState);
        }

        // Transition to Idle State
        else if (grounded)
        {
            playerController.JumpingCooldown();
            playerController.TransitionToState(playerController.IdleState);
        }
            
    }
}
