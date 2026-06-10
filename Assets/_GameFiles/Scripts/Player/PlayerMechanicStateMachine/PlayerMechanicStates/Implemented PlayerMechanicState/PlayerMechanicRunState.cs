using UnityEngine;

public sealed class PlayerMechanicRunState : PlayerMechanicState
{
    private readonly Transform _thirdPersonCameraControllerPivot;

    private readonly Vector2 _inputDirection;

    private readonly Vector2 _renderAndSkeletonRender;

    public PlayerMechanicRunState(Transform thirdPersonCameraControllerPivot, Vector2 inputDirection, Vector2 renderAndSkeletonPivot)
    {
        _thirdPersonCameraControllerPivot = thirdPersonCameraControllerPivot;
        _inputDirection = inputDirection;
        _renderAndSkeletonRender = renderAndSkeletonPivot;
    }

    public override void Enter(Player player)
    {
        //
    }

    public override void Do(Player player)
    {
        player.MovementController.Run(_thirdPersonCameraControllerPivot, _inputDirection, _renderAndSkeletonRender);
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
