using UnityEngine;

public sealed class CharacterMovementController
{
    private readonly CharacterLocomotion _locomotion;

    private readonly CharacterRotation _rotation;

    private readonly CharacterJump _jump;

    public CharacterMovementController(CharacterLocomotion locomotion, CharacterRotation rotation)
    {
        _locomotion = locomotion;
        _rotation = rotation;
    }
    
    public void Locomote(/*Transform cameraPoint, */Vector2 inputDirection)
    {
        //_locomotion.Locomote(CalculateWorldDirection(cameraPoint, inputDirection)); //œ≈–≈œ»—¿“‹
        _locomotion.Locomote(inputDirection); //œ≈–≈œ»—¿“‹
    }

    public void Run(/*Transform cameraPoint, */Vector2 inputDirection)
    {
        _locomotion.Run(inputDirection); //œ≈–≈œ»—¿“‹
    }

    public void Rotate(/*Transform cameraPoint, */Vector2 inputDirection)
    {
        _rotation.Rotate(inputDirection);
    }

    public void Jump()
    {
        //
    }

    private Vector3 CalculateWorldDirection(Transform cameraPoint, Vector2 inputDirection)
    {
        Vector3 cameraForward = new Vector3(cameraPoint.forward.x, 0f, cameraPoint.forward.z).normalized; //Ã√
        Vector3 cameraRight = new Vector3(cameraPoint.right.x, 0f, cameraPoint.right.z).normalized; //Ã√
        Vector3 worldDirection = (cameraForward * inputDirection.y + cameraRight * inputDirection.x).normalized;

        return worldDirection;
    }
}
