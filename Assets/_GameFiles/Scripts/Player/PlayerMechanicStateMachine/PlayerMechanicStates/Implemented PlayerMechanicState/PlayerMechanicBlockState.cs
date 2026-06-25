public class PlayerMechanicBlockState : PlayerMechanicState
{
    public override void Enter(Player player, PlayerMechanicStateMachine stateMachine)
    {
        //
    }

    public override void Do(Player player, PlayerMechanicStateMachine stateMachine)
    {
        player.HealthController.Health.Block();
    }

    public override void DoWithinFrame(Player player, PlayerMechanicStateMachine stateMachine)
    {
        //
    }

    public override bool TryExit(Player player, PlayerMechanicStateMachine stateMachine, PlayerMechanicState nextState)
    {
        if (nextState.GetType() == typeof(PlayerMechanicIdleState))
        {
            stateMachine.SwitchState(player, nextState);

            return true;
        }
        else
        {
            return false;
        }
    }
}
