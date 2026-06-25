public abstract class PlayerMechanicState
{
    public abstract void Enter(Player player, PlayerMechanicStateMachine stateMachine);

    public abstract void Do(Player player, PlayerMechanicStateMachine stateMachine);

    public abstract void DoWithinFrame(Player player, PlayerMechanicStateMachine stateMachine);

    public abstract bool TryExit(Player player, PlayerMechanicStateMachine stateMachine, PlayerMechanicState nextState);
}
