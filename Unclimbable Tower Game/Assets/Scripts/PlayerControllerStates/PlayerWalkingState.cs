using UnityEngine;
public class PlayerWalkingState : PlayerBaseState
{
    Vector3 moveDirection;
    bool grounded, running, walking, jumping;
    public override void EnterState(PlayerController_FSM playerController)
    {
        // Movement Direction
        moveDirection = playerController.GetMovementInputs();
    }

    public override void Update(PlayerController_FSM playerController)
    {
        // Movement Direction
        moveDirection = playerController.GetMovementInputs();

        // Booleans
        jumping = Input.GetButton("Jump") && playerController.Grounded && playerController.JumpingEnabled;
        walking = moveDirection.magnitude > 0;
        running = walking && Input.GetButton("Run");
        grounded = playerController.Grounded;
    }

    public override void FixedUpdate(PlayerController_FSM playerController)
    {
        // Actions
        playerController.Walk(moveDirection);
        playerController.ApplyDrag(playerController.Drag);
    }

    public override void LateUpdate(PlayerController_FSM playerController)
    {
        // Transition to Falling State
        if (!grounded)
            playerController.TransitionToState(playerController.FallingState);

        // Transition to Jumping State
        else if (jumping)
            playerController.TransitionToState(playerController.JumpingState);

        // Transition to Running State
        else if (running)
            playerController.TransitionToState(playerController.RunningState);

        // Transition to Idle State
        else if (!walking)
            playerController.TransitionToState(playerController.IdleState);
    }
}
