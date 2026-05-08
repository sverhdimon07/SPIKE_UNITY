using UnityEngine;

public class CharacterMovementController
{
    private readonly CharacterRotation _rotation = new CharacterRotation();

    private readonly CharacterLocomotion _locomotion = new CharacterLocomotion();

    private readonly CharacterJump _jump;

    public void Initialize(float locomotionSpeed)
    {
        _locomotion.Initialize(locomotionSpeed);
        //_running.Initialize(runningSpeed);
    }

    /*
    public void Rotate()
    {

    }*/

    public void LocomoteWithinFrame(Transform playerPoint, Vector2 locomotionDirection)
    {
        _locomotion.LocomoteWithinFrame(playerPoint, locomotionDirection);
    }

    /*
    public void Run(Transform playerPoint, Transform playerRenderAndSkeletonPoint, Vector2 locomotionDirection) //DI в Locomote и Run реализую потом
    {
        //_running.Run(playerPoint, playerRenderAndSkeletonPoint, locomotionDirection);
    }*/

    public void Jump()
    {
        //
    }
}
