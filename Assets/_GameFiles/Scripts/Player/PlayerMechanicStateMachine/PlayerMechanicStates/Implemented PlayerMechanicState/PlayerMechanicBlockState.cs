using UnityEngine;

public class PlayerMechanicBlockState : PlayerMechanicState
{
    public override void Enter(Player player)
    {
        //
    }

    public override void Do(Player player)
    {
        player.HealthController.Health.Block();
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
