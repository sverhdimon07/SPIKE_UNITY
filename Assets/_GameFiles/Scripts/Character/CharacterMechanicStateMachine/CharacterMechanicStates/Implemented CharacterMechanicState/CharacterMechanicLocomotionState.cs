using UnityEngine;

public sealed class CharacterMechanicLocomotionState : CharacterMechanicState
{
    //private readonly Transform _thirdPersonCameraControllerPivot;

    private readonly Vector2 _inputDirection;
    
    public CharacterMechanicLocomotionState(/*Transform thirdPersonCameraControllerPivot, */Vector2 inputDirection)
    {
        //_thirdPersonCameraControllerPivot = thirdPersonCameraControllerPivot;
        _inputDirection = inputDirection;
    }

    public override void Enter(Character character)
    {
        //
    }

    public override void Do(Character character)
    {
        character.MovementController.Locomote(/*_thirdPersonCameraControllerPivot, */_inputDirection);
    }

    public override void DoWithinFrame(Character character)
    {
        //
    }

    public override void Exit(Character character)
    {
        //
    }
}
