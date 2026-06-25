public sealed class PlayerMechanicStunState : PlayerMechanicState
{
    private readonly float _damage;

    public PlayerMechanicStunState(float damage)
    {
        _damage = damage;
    }

    public override void Enter(Player player, PlayerMechanicStateMachine stateMachine)
    {
        //
    }

    public override void Do(Player player, PlayerMechanicStateMachine stateMachine)
    {
        player.HealthController.Health.TakeDamage(_damage);
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
        else if (nextState.GetType() == typeof(PlayerMechanicDeathState))
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
