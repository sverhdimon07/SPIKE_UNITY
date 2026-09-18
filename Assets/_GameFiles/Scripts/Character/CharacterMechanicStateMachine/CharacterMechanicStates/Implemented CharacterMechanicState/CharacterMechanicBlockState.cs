using UnityEngine;

public class CharacterMechanicBlockState : CharacterMechanicState
{
    public override void Enter(Character character, CharacterMechanicStateMachine stateMachine)
    {
        //
    }

    public override void Do(Character character, CharacterMechanicStateMachine stateMachine)
    {
        character.HealthController.Health.Block();
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
        else
        {
            return false;
        }
    }
}
