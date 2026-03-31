using UnityEngine;

public sealed class PlayerMovementController
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

    public void LocomoteWithinFrame(Transform playerPoint, Transform playerRenderAndSkeletonPoint, Vector2 locomotionDirection, Transform cameraPoint)
    {
        _locomotion.LocomoteWithinFrame(playerPoint, playerRenderAndSkeletonPoint, locomotionDirection, cameraPoint);
    }

    public void RunWithinFrame(Transform playerPoint, Transform playerRenderAndSkeletonPoint, Vector2 locomotionDirection, Transform cameraPoint) //DI в Locomote и Run реализую потом
    {
        _running.RunWithinFrame(playerPoint, playerRenderAndSkeletonPoint, locomotionDirection, cameraPoint);
    }

    public void Jump()
    {
        //
    }
}
