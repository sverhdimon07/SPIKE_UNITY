public sealed class CharacterMechanicIdleState : CharacterMechanicState
{
    public override void Enter(Character character)
    {
        //
    }

    public override void Do(Character character)
    {
        character.Idled.Invoke(); //можно создать контроллер для айдла
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
