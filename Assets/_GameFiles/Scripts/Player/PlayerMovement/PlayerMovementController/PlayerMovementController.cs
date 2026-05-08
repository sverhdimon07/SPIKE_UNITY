using UnityEngine;

public sealed class PlayerMovementController
{
    private readonly PlayerRotation _rotation = new PlayerRotation();

    private readonly PlayerLocomotion _locomotion = new PlayerLocomotion();

    private readonly PlayerJump _jump;

    public void Initialize(float locomotionSpeed, float runningSpeed)
    {
        _locomotion.Initialize(locomotionSpeed, runningSpeed);
    }

    public void RotateWithinFrame(Transform renderAndSkeletonPoint, Transform cameraPoint, Vector2 inputDirection)
    {
        _rotation.RotateWithinFrame(renderAndSkeletonPoint, CalculateRequiredActualDirection(cameraPoint, inputDirection));
    }

    public void LocomoteWithinFrame(Transform point, Transform cameraPoint, Vector2 inputLocomotionDirection, bool isRunning)
    {
        if (isRunning == false)
        {
            _locomotion.LocomoteWithinFrame(point, CalculateRequiredActualDirection(cameraPoint, inputLocomotionDirection));
        }
        else if (isRunning == true)
        {
            _locomotion.RunWithinFrame(point, CalculateRequiredActualDirection(cameraPoint, inputLocomotionDirection));
        }
    }
    
    public void Jump()
    {
        //
    }

    private Vector3 CalculateRequiredActualDirection(Transform cameraPoint, Vector2 inputDirection)
    {
        Vector3 cameraForward = new Vector3(cameraPoint.forward.x, 0f, cameraPoint.forward.z).normalized; //Ã√
        Vector3 cameraRight = new Vector3(cameraPoint.right.x, 0f, cameraPoint.right.z).normalized; //Ã√
        Vector3 requiredActualDirection = (cameraForward * inputDirection.y + cameraRight * inputDirection.x).normalized;

        return requiredActualDirection;
    }
}
