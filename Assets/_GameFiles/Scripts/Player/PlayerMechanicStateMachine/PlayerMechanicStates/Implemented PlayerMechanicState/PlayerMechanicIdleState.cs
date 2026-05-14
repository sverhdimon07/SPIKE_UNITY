public sealed class PlayerMechanicIdleState : PlayerMechanicState
{
    public void RefreshData() //в параметре пока данные не нужны для этого стейта
    {
        //
    }

    public override void Enter(Player player)
    {
        player.Idle();
    }

    public override void DoLogic(Player player)
    {
        //
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
