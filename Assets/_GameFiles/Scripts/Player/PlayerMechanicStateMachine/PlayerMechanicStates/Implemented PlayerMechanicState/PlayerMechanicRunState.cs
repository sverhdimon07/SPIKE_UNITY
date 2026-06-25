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

    public override void Enter(Player player, PlayerMechanicStateMachine stateMachine)
    {
        //
    }

    public override void Do(Player player, PlayerMechanicStateMachine stateMachine)
    {
        player.MovementController.Run(_thirdPersonCameraControllerPivot, _inputDirection, _renderAndSkeletonRender);
    }

    public override void DoWithinFrame(Player player, PlayerMechanicStateMachine stateMachine)
    {
        //
    }

    public override bool TryExit(Player player, PlayerMechanicStateMachine stateMachine, PlayerMechanicState nextState)
    {
        stateMachine.SwitchState(player, nextState);

        return true;
    }
}
