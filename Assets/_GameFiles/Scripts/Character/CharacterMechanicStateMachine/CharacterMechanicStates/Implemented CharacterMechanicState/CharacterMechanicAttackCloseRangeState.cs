using UnityEngine;

public sealed class CharacterMechanicAttackCloseRangeState : CharacterMechanicState
{
    private readonly Vector3 _gameObjectPosition;

    private readonly Vector2 _renderAndSkeletonDirectionXZ;

    public CharacterMechanicAttackCloseRangeState(Vector3 gameObjectPosition, Vector2 renderAndSkeletonDirectionXZ)
    {
        _gameObjectPosition = gameObjectPosition;
        _renderAndSkeletonDirectionXZ = renderAndSkeletonDirectionXZ;
    }

    public override void Enter(Character character)
    {
        //
    }
    
    public override void Do(Character character)
    {
        character.OffenseController.AttackCloseRange(_gameObjectPosition, _renderAndSkeletonDirectionXZ);
    }

    public override void DoWithinFrame(Character character)
    {
        //
    }

    public override void Exit(Character character)
    {
        //
    }
}
