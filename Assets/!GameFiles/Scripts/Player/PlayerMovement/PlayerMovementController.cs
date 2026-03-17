using UnityEngine;

public class PlayerMovementController
{
    private readonly PlayerRotation _rotation;

    private readonly PlayerLocomotion _locomotion = new PlayerLocomotion();

    private readonly PlayerRunning _running = new PlayerRunning();

    private readonly PlayerJump _jump;

    public void Initialize(float locomotionSpeed, float runningSpeed)
    {
        _locomotion.Initialize(locomotionSpeed);
        _running.Initialize(runningSpeed);
    }

    public void Rotate()
    {

    }

    public void Locomote(Transform playerPoint, Transform playerRenderAndSkeletonPoint, Vector2 locomotionDirection)
    {
        _locomotion.Locomote(playerPoint, playerRenderAndSkeletonPoint, locomotionDirection);
    }

    public void Run(Transform playerPoint, Transform playerRenderAndSkeletonPoint, Vector2 locomotionDirection) //DI в Locomote и Run реализую потом
    {
        _running.Run(playerPoint, playerRenderAndSkeletonPoint, locomotionDirection);
    }

    public void Jump()
    {
        //
    }
}
