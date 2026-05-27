using UnityEngine;

public sealed class PlayerMechanicLocomotionState : PlayerMechanicState
{
    private readonly Transform _thirdPersonCameraControllerPivot;

    private readonly Vector2 _inputDirection;
    
    public PlayerMechanicLocomotionState(Transform thirdPersonCameraControllerPivot, Vector2 inputDirection)
    {
        _thirdPersonCameraControllerPivot = thirdPersonCameraControllerPivot;
        _inputDirection = inputDirection;
    }

    public override void Enter(Player player)
    {
        //
    }

    public override void Do(Player player)
    {
        player.MovementController.Locomote(_thirdPersonCameraControllerPivot, _inputDirection);
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
