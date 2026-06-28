using UnityEngine;

public sealed class CharacterMechanicAttackLongRangeState : CharacterMechanicState
{
    private readonly Vector3 _gameObjectPosition;

    private readonly Vector2 _renderAndSkeletonDirectionXZ;

    public CharacterMechanicAttackLongRangeState(Vector3 gameObjectPosition, Vector2 renderAndSkeletonDirectionXZ)
    {
        _gameObjectPosition = gameObjectPosition;
        _renderAndSkeletonDirectionXZ = renderAndSkeletonDirectionXZ;
    }

    public override void Enter(Character character, CharacterMechanicStateMachine stateMachine)
    {
        //
    }

    public override void Do(Character character, CharacterMechanicStateMachine stateMachine)
    {
        character.OffenseController.AttackLongRange(_gameObjectPosition, _renderAndSkeletonDirectionXZ);
    }

    public override void DoWithinFrame(Character character, CharacterMechanicStateMachine stateMachine)
    {
        //
    }

    public override bool TryExit(Character character, CharacterMechanicStateMachine stateMachine, CharacterMechanicState nextState)
    {
        if (nextState.GetType() == typeof(CharacterMechanicIdleState))
        {
            stateMachine.SwitchState(character, nextState);

            return true;
        }
        else if (nextState.GetType() == typeof(CharacterMechanicStunState))
        {
            stateMachine.SwitchState(character, nextState);

            return true;
        }
        else if (nextState.GetType() == typeof(CharacterMechanicDeathState))
        {
            stateMachine.SwitchState(character, nextState);

            return true;
        }
        else
        {
            return false;
        }
    }
}
