public sealed class CharacterMechanicStunState : CharacterMechanicState
{
    private readonly float _damage;

    public CharacterMechanicStunState(float damage)
    {
        _damage = damage;
    }

    public override void Enter(Character character)
    {
        //
    }

    public override void Do(Character character)
    {
        character.HealthController.Health.TakeDamage(_damage);
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
