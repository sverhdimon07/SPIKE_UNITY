using UnityEngine;

public sealed class PlayerMechanicAttackCloseRangeState : PlayerMechanicState
{
    private readonly Vector3 _gameObjectPosition;

    private readonly Vector2 _renderAndSkeletonDirectionXZ;

    public PlayerMechanicAttackCloseRangeState(Vector3 gameObjectPosition, Vector2 renderAndSkeletonDirectionXZ)
    {
        _gameObjectPosition = gameObjectPosition;
        _renderAndSkeletonDirectionXZ = renderAndSkeletonDirectionXZ;
    }

    public override void Enter(Player player, PlayerMechanicStateMachine stateMachine)
    {
        //
    }
    
    public override void Do(Player player, PlayerMechanicStateMachine stateMachine)
    {
        player.OffenseController.AttackCloseRange(_gameObjectPosition, _renderAndSkeletonDirectionXZ);
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
        else if (nextState.GetType() == typeof(PlayerMechanicStunState))
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
