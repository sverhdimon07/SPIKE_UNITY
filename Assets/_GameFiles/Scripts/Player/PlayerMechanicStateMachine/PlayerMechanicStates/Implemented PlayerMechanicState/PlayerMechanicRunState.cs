using UnityEngine;

public sealed class PlayerMechanicRunState : PlayerMechanicState
{
    public override void Enter(Player player)
    {
        //
    }

    public override void DoLogic(Player player)
    {
        player.MovementController.Run(player.ThirdPersonCameraControllerPivot, player.InputDirection);
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
