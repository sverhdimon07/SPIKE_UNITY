public sealed class CharacterMechanicDeathState : CharacterMechanicState
{
    public override void Enter(Character character)
    {
        //
    }

    public override void Do(Character character)
    {
        character.HealthController.Health.Die();
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
