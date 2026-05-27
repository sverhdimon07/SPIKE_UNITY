public sealed class PlayerMechanicStunState : PlayerMechanicState
{
    private readonly float _damage;

    public PlayerMechanicStunState(float damage)
    {
        _damage = damage;
    }

    public override void Enter(Player player)
    {
        //
    }

    public override void Do(Player player)
    {
        player.HealthController.Health.TakeDamage(_damage);
    }

    public override void DoWithinFrame(Player player)
    {
        //
    }

    public override void Exit(Player player)
    {
        //
    }
}
