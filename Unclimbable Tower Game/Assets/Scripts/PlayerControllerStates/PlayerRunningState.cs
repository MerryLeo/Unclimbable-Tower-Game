using UnityEngine;
public class PlayerRunningState : PlayerBaseState
{
    Vector3 moveDirection;
    bool grounded, jumping, walking, running;
    public override void EnterState(PlayerController_FSM playerController)
    {
        // Movement Direction
        moveDirection = playerController.GetMovementInputs();

        // Change the camera FOV
        playerController.CameraControllerScript.SetFOV(playerController.RunningFOV);
    }
    public override void Update(PlayerController_FSM playerController)
    {
        // Movement Direction
        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalAxis = Input.GetAxis("Horizontal");
        //moveDirection = verticalAxis > 0 ? verticalAxis * playerController.CameraTransform.forward : verticalAxis * playerController.CameraTransform.forward * playerController.BackwardRunningFactor; // Forward Movement
        //moveDirection += horizontalAxis * playerController.CameraTransform.right * playerController.HorizontalRunningFactor; // Horizontal Movement
        moveDirection = playerController.GetMovementInputs(playerController.HorizontalRunningFactor, 1);

        // Booleans
        grounded = playerController.Grounded;
        walking = moveDirection.magnitude > 0 && !Input.GetButton("Run");
        running = moveDirection.magnitude > 0 && Input.GetButton("Run");
        jumping = Input.GetButton("Jump") && playerController.Grounded && playerController.JumpingEnabled;
    }

    public override void FixedUpdate(PlayerController_FSM playerController)
    {
        // Actions
        playerController.Run(moveDirection);
        playerController.ApplyDrag(playerController.Drag);
    }

    public override void LateUpdate(PlayerController_FSM playerController)
    {
        // Transition to Falling State
        if (!grounded)
        {
            playerController.TransitionToState(playerController.FallingState);
            playerController.CameraControllerScript.ResetFOV();
        }   

        // Transition to Jumping State
        else if (jumping)
        {
            playerController.TransitionToState(playerController.JumpingState);
            playerController.CameraControllerScript.ResetFOV();
        }

        // Transition to Walking State
        else if (walking)
        {
            playerController.TransitionToState(playerController.WalkingState);
            playerController.CameraControllerScript.ResetFOV();
        }
            
        // Transition to Idle State
        else if (!running)
        {
            playerController.TransitionToState(playerController.IdleState);
            playerController.CameraControllerScript.ResetFOV();
        }
    }


}
