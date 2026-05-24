public sealed class PlayerMechanicIdleState : PlayerMechanicState
{
    public override void Enter(Player player)
    {
        //
    }

    public override void DoLogic(Player player)
    {
        player.StartedToIdle.Invoke();
    }

    public override void DoLogicWithinFrame(Player player)
    {
        //
    }

    public override void Exit(Player player)
    {
        //
    }
}
