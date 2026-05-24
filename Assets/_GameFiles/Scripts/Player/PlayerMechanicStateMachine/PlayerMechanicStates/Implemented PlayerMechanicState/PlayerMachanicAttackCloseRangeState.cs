public class PlayerMachanicAttackCloseRangeState : PlayerMechanicState
{
    public override void Enter(Player player)
    {
        //
    }

    public override void DoLogic(Player player)
    {
        player.OffenseController.AttackLongRange(player.GameObjectPosition, player.RenderAndSkeletonDirectionXZ);
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
