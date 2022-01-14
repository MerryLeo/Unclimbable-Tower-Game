using UnityEngine;
public class PlayerIdleState : PlayerBaseState
{
    bool grounded, walking, running, jumping;
    public override void EnterState(PlayerController_FSM playerController) 
    {
        
    }

    public override void Update(PlayerController_FSM playerController)
    {
        // Booleans
        grounded = playerController.Grounded;
        walking = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        running = walking && Input.GetButton("Run");
        jumping = Input.GetButton("Jump") && playerController.Grounded && playerController.JumpingEnabled;
    }
    public override void FixedUpdate(PlayerController_FSM playerController) 
    { 
        // Action
        if (playerController.CurrentVelocity.magnitude > 0)
        {
            playerController.ApplyDrag(playerController.Drag);
        }
    }

    public override void LateUpdate(PlayerController_FSM playerController)
    {
        // Transition to Falling State
        if (!grounded)
        {
            playerController.TransitionToState(playerController.FallingState);
        }
        
        // Transition to Jumping State
        if (jumping) 
        {
            playerController.TransitionToState(playerController.JumpingState);
        }

        // Transition to Running State
        else if (running)
        {
            playerController.TransitionToState(playerController.RunningState);
        }
            
        // Transition to Walking State
        else if (walking) 
        {
            playerController.TransitionToState(playerController.WalkingState); 
        }
            
    }
}
