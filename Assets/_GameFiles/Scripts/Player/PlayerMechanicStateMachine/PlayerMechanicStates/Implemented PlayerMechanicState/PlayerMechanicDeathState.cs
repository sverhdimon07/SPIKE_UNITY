public sealed class PlayerMechanicDeathState : PlayerMechanicState
{
    public override void Enter(Player player, PlayerMechanicStateMachine stateMachine)
    {
        //
    }

    public override void Do(Player player, PlayerMechanicStateMachine stateMachine)
    {
        player.HealthController.Health.Die();
    }

    public override void DoWithinFrame(Player player, PlayerMechanicStateMachine stateMachine)
    {
        //
    }

    public override bool TryExit(Player player, PlayerMechanicStateMachine stateMachine, PlayerMechanicState nextState)
    {
        return false;
    }
}
