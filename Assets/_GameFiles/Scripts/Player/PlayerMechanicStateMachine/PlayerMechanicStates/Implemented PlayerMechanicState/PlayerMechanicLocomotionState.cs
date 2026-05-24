using UnityEngine;

public sealed class PlayerMechanicLocomotionState : PlayerMechanicState
{
    public override void Enter(Player player)
    {
        //
    }

    public override void DoLogic(Player player)
    {
        player.MovementController.Locomote(player.ThirdPersonCameraControllerPivot, player.InputDirection);
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
