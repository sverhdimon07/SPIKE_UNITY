public abstract class CharacterMechanicState
{
    public abstract void Enter(Character character, CharacterMechanicStateMachine stateMachine);

    public abstract void Do(Character character, CharacterMechanicStateMachine stateMachine);

    public abstract void DoWithinFrame(Character character, CharacterMechanicStateMachine stateMachine);

    public abstract bool TryExit(Character character, CharacterMechanicStateMachine stateMachine, CharacterMechanicState nextState);
}
