public sealed class PlayerMechanicDeathState : PlayerMechanicState
{
    public override void Enter(Player player)
    {
        //
    }

    public override void Do(Player player)
    {
        player.HealthController.Health.Die();
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
