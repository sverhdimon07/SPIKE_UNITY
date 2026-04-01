using UnityEngine;

public class CharacterMovementController
{
    private readonly CharacterRotation _rotation = new CharacterRotation();

    private readonly CharacterLocomotion _locomotion = new CharacterLocomotion();

    private readonly CharacterJump _jump;

    public void Initialize(float locomotionSpeed, float runningSpeed)
    {
        _locomotion.Initialize(locomotionSpeed);
        //_running.Initialize(runningSpeed);
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
        //_running.Run(playerPoint, playerRenderAndSkeletonPoint, locomotionDirection);
    }

    public void Jump()
    {
        //
    }
}
