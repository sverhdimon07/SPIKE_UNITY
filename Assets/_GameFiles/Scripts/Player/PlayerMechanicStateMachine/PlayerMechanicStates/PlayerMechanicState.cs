public abstract class PlayerMechanicState
{
    public PlayerMechanicStateMachine StateMachine;
    
    public abstract void Enter();

    public abstract void DoDomainLogic();

    public abstract void DoDomainLogicWithinFrame();

    public abstract void Exit();
}
