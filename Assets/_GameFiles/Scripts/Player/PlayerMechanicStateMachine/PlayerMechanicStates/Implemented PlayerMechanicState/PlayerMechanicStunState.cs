using UnityEngine;

public sealed class PlayerMechanicStunState : PlayerMechanicState
{
    private const float RecoveryTimeout = 1.35f;

    private readonly float _damage;
    private double _recoveryDeadline;

    public PlayerMechanicStunState(float damage)
    {
        _damage = damage;
    }

    public override void Enter(Player player, PlayerMechanicStateMachine stateMachine)
    {
        //
        _recoveryDeadline = Time.timeAsDouble + RecoveryTimeout;
    }

    public override void Do(Player player, PlayerMechanicStateMachine stateMachine)
    {
        player.HealthController.Health.TakeDamage(_damage);
    }

    public override void DoWithinFrame(Player player, PlayerMechanicStateMachine stateMachine)
    {
        //
        if (Time.timeAsDouble >= _recoveryDeadline)
        {
            player.Idle();
        }
    }

    public override bool TryExit(Player player, PlayerMechanicStateMachine stateMachine, PlayerMechanicState nextState)
    {
        if (nextState is PlayerMechanicIdleState or PlayerMechanicFlightState)
        {
            stateMachine.SwitchState(player, nextState);

            return true;
        }
        else if (nextState.GetType() == typeof(PlayerMechanicDeathState))
        {
            stateMachine.SwitchState(player, nextState);

            return true;
        }
        else if (nextState.GetType() == typeof(PlayerMechanicStunState))
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
