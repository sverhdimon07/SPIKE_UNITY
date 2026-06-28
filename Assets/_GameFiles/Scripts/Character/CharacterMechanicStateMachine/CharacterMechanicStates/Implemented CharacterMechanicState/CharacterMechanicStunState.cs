using UnityEngine;

public sealed class CharacterMechanicStunState : CharacterMechanicState
{
    private readonly float _damage;

    public CharacterMechanicStunState(float damage)
    {
        _damage = damage;
    }

    public override void Enter(Character character, CharacterMechanicStateMachine stateMachine)
    {
        //
    }

    public override void Do(Character character, CharacterMechanicStateMachine stateMachine)
    {
        character.HealthController.Health.TakeDamage(_damage);
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
