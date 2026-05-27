public abstract class CharacterMechanicState
{
    public abstract void Enter(Character character);

    public abstract void Do(Character character);

    public abstract void DoWithinFrame(Character character);

    public abstract void Exit(Character character);
}
