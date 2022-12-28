using UnityEngine;
public class PlayerFallingState : PlayerBaseState 
{
    Vector3 _moveDirection;
    bool _grounded;
    public override void EnterState(PlayerController playerController) { }

    public override void Update(PlayerController playerController) 
    {
        // Movement Direction
        _moveDirection = playerController.GetMovementInputs();

        // Boolean
        _grounded = playerController.Grounded;
    }

    public override void FixedUpdate(PlayerController playerController) 
    { 
        playerController.MoveMidair(_moveDirection);
        playerController.ApplyDrag(playerController.MidairDrag);
    }

    public override void LateUpdate(PlayerController playerController) 
    {
        // Transition to Idle State
        if (_grounded) 
        {
            playerController.JumpingCooldown();
            playerController.TransitionToState(playerController.IdleState);
        }
    }
}
