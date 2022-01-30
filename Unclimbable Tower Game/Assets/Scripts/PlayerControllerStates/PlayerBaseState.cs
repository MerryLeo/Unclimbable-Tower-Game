public abstract class PlayerBaseState {
    public abstract void EnterState(PlayerController_FSM playerController);
    public abstract void Update(PlayerController_FSM playerController);
    public abstract void FixedUpdate(PlayerController_FSM playerController);
    public abstract void LateUpdate(PlayerController_FSM playerController);
}
