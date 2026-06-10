using UnityEngine;

public sealed class PlayerMovementController
{
    private readonly PlayerLocomotion _locomotion;

    private readonly PlayerRotation _rotation;

    private readonly PlayerJump _jump;

    public PlayerMovementController(PlayerLocomotion locomotion, PlayerRotation rotation)
    {
        _locomotion = locomotion;
        _rotation = rotation;
    }
    
    public void Locomote(Transform cameraPoint, Vector2 inputDirection, Vector2 skeletonAndRenderDirection)
    {
        _locomotion.Locomote(CalculateWorldDirection(cameraPoint, inputDirection), skeletonAndRenderDirection); //œ≈–≈œ»—¿“‹
    }

    public void Run(Transform cameraPoint, Vector2 inputDirection, Vector2 skeletonAndRenderDirection)
    {
        _locomotion.Run(CalculateWorldDirection(cameraPoint, inputDirection), skeletonAndRenderDirection); //œ≈–≈œ»—¿“‹
    }

    public void Rotate(Transform cameraPoint, Vector2 inputDirection)
    {
        _rotation.Rotate(CalculateWorldDirection(cameraPoint, inputDirection));
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
