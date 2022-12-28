public abstract class PlayerBaseState 
{
    public abstract void EnterState(PlayerController playerController);
    public abstract void Update(PlayerController playerController);
    public abstract void FixedUpdate(PlayerController playerController);
    public abstract void LateUpdate(PlayerController playerController);
}
