using UnityEngine;
public class PlayerFallingState : PlayerBaseState
{
    Vector3 moveDir;
    bool grounded;
    public override void EnterState(PlayerController_FSM playerController) { }

    public override void Update(PlayerController_FSM playerController)
    {
        // Movement Direction
        moveDir = playerController.GetMovementInputs();


        // Boolean
        grounded = playerController.Grounded;
    }

    public override void FixedUpdate(PlayerController_FSM playerController) 
    { 
        playerController.MoveMidair(moveDir);
        playerController.ApplyDrag(playerController.MidairDrag);
    }

    public override void LateUpdate(PlayerController_FSM playerController)
    {
        // Transition to Idle State
        if (grounded)
        {
            playerController.JumpingCooldown();
            playerController.TransitionToState(playerController.IdleState);
        }
    }
}
