public class PlayerMachanicAttackLongRangeState : PlayerMechanicState
{
    public override void Enter(Player player)
    {
        //
    }

    public override void DoLogic(Player player)
    {
        player.OffenseController.AttackCloseRange(player.GameObjectPosition, player.RenderAndSkeletonDirectionXZ);
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
