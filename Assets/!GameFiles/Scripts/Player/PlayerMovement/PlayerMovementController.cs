using UnityEngine; //нужен ли MonoBehaviour?

public class PlayerMovementController
{
    private PlayerRotation _rotation;

    private PlayerLocomotion _locomotion = new PlayerLocomotion();

    private PlayerRunning _running = new PlayerRunning();

    private PlayerJump _jump;

    public void Initialize(float speed)
    {
        InitializeLocomotion(speed);
        InitializeRunning(speed);
    }

    public void Rotate()
    {

    }

    public void Locomote(Transform point, Vector2 direction)
    {
        _locomotion.Locomote(point, direction);
    }

    public void Run(Transform point, Vector2 direction) //DI в Locomote и Run реализую потом
    {
        _running.Run(point, direction);
    }

    public void Jump()
    {
        //
    }

    private void InitializeLocomotion(float speed)
    {
        _locomotion.Initialize(speed);
    }

    private void InitializeRunning(float speed)
    {
        _running.Initialize(speed);
    }
}
