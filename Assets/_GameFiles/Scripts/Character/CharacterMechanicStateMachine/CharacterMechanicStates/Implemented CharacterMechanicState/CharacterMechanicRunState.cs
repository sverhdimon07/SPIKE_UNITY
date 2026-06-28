using UnityEngine;
using UnityEngine.TextCore.Text;

public sealed class CharacterMechanicRunState : CharacterMechanicState
{
    //private readonly Transform _thirdPersonCameraControllerPivot;

    private readonly Vector2 _inputDirection;

    public CharacterMechanicRunState(/*Transform thirdPersonCameraControllerPivot, */Vector2 inputDirection)
    {
        //_thirdPersonCameraControllerPivot = thirdPersonCameraControllerPivot;
        _inputDirection = inputDirection;
    }

    public override void Enter(Character character, CharacterMechanicStateMachine stateMachine)
    {
        //
    }

    public override void Do(Character character, CharacterMechanicStateMachine stateMachine)
    {
        character.MovementController.Run(/*_thirdPersonCameraControllerPivot, */_inputDirection);
    }

    public override void DoWithinFrame(Character character, CharacterMechanicStateMachine stateMachine)
    {
        //
    }

    public override bool TryExit(Character character, CharacterMechanicStateMachine stateMachine, CharacterMechanicState nextState)
    {
        stateMachine.SwitchState(character, nextState);

        return true;
    }
}
