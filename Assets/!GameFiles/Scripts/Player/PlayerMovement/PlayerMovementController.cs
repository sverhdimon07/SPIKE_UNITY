using UnityEngine;

public class PlayerMovementController
{
    private PlayerRotation _rotation;

    private PlayerLocomotion _locomotion = new PlayerLocomotion();

    private PlayerRunning _running = new PlayerRunning();

    private PlayerJump _jump;

    public void Initialize(float speed)
    {
        _locomotion.Initialize(speed);
        _running.Initialize(speed);
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
