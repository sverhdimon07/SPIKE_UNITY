public sealed class PlayerMechanicFlightState : PlayerMechanicState
{
    public override void Enter(Player player, PlayerMechanicStateMachine stateMachine)
    {
        player.HealthController.Health.Unblock();
    }

    public override void Do(Player player, PlayerMechanicStateMachine stateMachine)
    {
    }

    public override void DoWithinFrame(Player player, PlayerMechanicStateMachine stateMachine)
    {
    }

    public override bool TryExit(Player player, PlayerMechanicStateMachine stateMachine, PlayerMechanicState nextState)
    {
        if (nextState is not PlayerMechanicIdleState)
        {
            return false;
        }

        stateMachine.SwitchState(player, nextState);
        return true;
    }
}
