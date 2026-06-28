public sealed class CharacterMechanicIdleState : CharacterMechanicState
{
    public override void Enter(Character character, CharacterMechanicStateMachine stateMachine)
    {
        //
    }

    public override void Do(Character character, CharacterMechanicStateMachine stateMachine)
    {
        character.Idled.Invoke(); //можно создать контроллер для айдла
    }

    public override void DoWithinFrame(Character character, CharacterMechanicStateMachine stateMachine)
    {
        //
    }

    public override bool TryExit(Character character, CharacterMechanicStateMachine stateMachine, CharacterMechanicState nextState)
    {
        stateMachine.SwitchState(character, nextState);

        return true;
    }
}
