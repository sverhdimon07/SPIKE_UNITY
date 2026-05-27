using UnityEngine;

public sealed class PlayerMechanicAttackLongRangeState : PlayerMechanicState
{
    private readonly Vector3 _gameObjectPosition;

    private readonly Vector2 _renderAndSkeletonDirectionXZ;

    public PlayerMechanicAttackLongRangeState(Vector3 gameObjectPosition, Vector2 renderAndSkeletonDirectionXZ)
    {
        _gameObjectPosition = gameObjectPosition;
        _renderAndSkeletonDirectionXZ = renderAndSkeletonDirectionXZ;
    }

    public override void Enter(Player player)
    {
        //
    }

    public override void Do(Player player)
    {
        player.OffenseController.AttackLongRange(_gameObjectPosition, _renderAndSkeletonDirectionXZ);
    }

    public override void DoWithinFrame(Player player)
    {
        //
    }

    public override void Exit(Player player)
    {
        //
    }
}
