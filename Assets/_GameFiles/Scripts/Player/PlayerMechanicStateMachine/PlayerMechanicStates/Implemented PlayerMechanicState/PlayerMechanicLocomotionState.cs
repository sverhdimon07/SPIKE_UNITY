using UnityEngine;

public sealed class PlayerMechanicLocomotionState : PlayerMechanicState
{
    private readonly Transform _thirdPersonCameraControllerPivot;

    private readonly Vector2 _inputDirection;

    private readonly Vector2 _renderAndSkeletonPivot;

    public PlayerMechanicLocomotionState(Transform thirdPersonCameraControllerPivot, Vector2 inputDirection, Vector2 renderAndSkeletonPivot)
    {
        _thirdPersonCameraControllerPivot = thirdPersonCameraControllerPivot;
        _inputDirection = inputDirection;
        _renderAndSkeletonPivot = renderAndSkeletonPivot;
    }

    public override void Enter(Player player, PlayerMechanicStateMachine stateMachine)
    {
        //
    }

    public override void Do(Player player, PlayerMechanicStateMachine stateMachine)
    {
        player.MovementController.Locomote(_thirdPersonCameraControllerPivot, _inputDirection, _renderAndSkeletonPivot);
    }

    public override void DoWithinFrame(Player player, PlayerMechanicStateMachine stateMachine)
    {
        //
    }

    public override bool TryExit(Player player, PlayerMechanicStateMachine stateMachine, PlayerMechanicState nextState)
    {
        stateMachine.SwitchState(player, nextState);

        return true;
        /*
        if (PlayerMechanicLocomotionState.ReferenceEquals == _state.GetType())
        {
            return;
        }*/
    }
}
