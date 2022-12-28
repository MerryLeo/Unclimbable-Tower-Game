using UnityEngine;
using System.Threading.Tasks;

public class PlayerJumpingState : PlayerBaseState 
{
    const float _fallingCooldown = 0.05f;
    Vector3 _moveDirection;
    bool _falling, _grounded, _checkIfFalling;

    public override void EnterState(PlayerController playerController) 
    {
        // Initial Action
        playerController.Jump();
        FallingCooldown();
    }

    public override void Update(PlayerController playerController) 
    {
        // Movement Direction
        _moveDirection = playerController.GetMovementInputs();

        // Boolean
        _falling = playerController.RigidBodyIsFalling && _checkIfFalling;
        _grounded = playerController.Grounded && !playerController.RigidBodyIsFalling && !playerController.RigidBodyIsRising;
    }
    public override void FixedUpdate(PlayerController playerController) 
    {
        // Action
        playerController.MoveMidair(_moveDirection);
        playerController.ApplyDrag(playerController.MidairDrag);
    }

    public override void LateUpdate(PlayerController playerController) 
    {
        // Transition to Falling State
        if (_falling) 
            playerController.TransitionToState(playerController.FallingState);
        
        // Transition to Idle State
        else if (_grounded) 
        { 
            playerController.JumpingCooldown();
            playerController.TransitionToState(playerController.IdleState);
        }
    }

    // Can Transition to falling state after this cooldown
    async void FallingCooldown() 
    {
        _checkIfFalling = false;
        float endTime = Time.time + _fallingCooldown;
        while (Time.time < endTime) 
        {
            await Task.Yield();
        }
        _checkIfFalling = true;
    }
}
