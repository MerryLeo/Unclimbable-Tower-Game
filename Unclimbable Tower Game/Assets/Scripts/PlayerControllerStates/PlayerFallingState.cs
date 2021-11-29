using UnityEngine;
public class PlayerFallingState : PlayerBaseState
{
    bool grounded;
    public override void EnterState(PlayerController_FSM playerController)
    {
    }

    public override void Update(PlayerController_FSM playerController)
    {
        // Boolean
        grounded = playerController.Grounded;
    }

    public override void FixedUpdate(PlayerController_FSM playerController)
    {
        // Action
        playerController.ApplyDrag(playerController.MidairDrag);
    }

    public override void LateUpdate(PlayerController_FSM playerController)
    {
        // Transition to Idle State
        if (grounded)
            playerController.TransitionToState(playerController.IdleState);
    }
}
