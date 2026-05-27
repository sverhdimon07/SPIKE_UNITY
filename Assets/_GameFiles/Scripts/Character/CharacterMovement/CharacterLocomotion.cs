using UnityEngine;
using UnityEngine.Events;

public sealed class CharacterLocomotion
{
    public static UnityAction<Vector3> Locomoted;
    public static UnityAction<Vector3> Runned;

    private readonly float _locomotionSpeed;
    private readonly float _runningSpeed;

    private Vector3 _lastPosition;

    public CharacterLocomotion(float locomotionSpeed, float runningSpeed, Vector3 lastPosition)
    {
        _locomotionSpeed = locomotionSpeed;
        _runningSpeed = runningSpeed;
        _lastPosition = lastPosition;
    }

    public void Locomote(Vector2 direction) //ИНКАПУСЛЯЦИЯ
    {
        Vector3 directionCalibrated = new Vector3(direction.x, 0f, direction.y);
        Vector3 nextPosition = _lastPosition += directionCalibrated * _locomotionSpeed * Time.deltaTime;
        //Debug.Log(Locomoted.GetInvocationList());
        Locomoted.Invoke(nextPosition);
        //Debug.Log("AAAAAAA");
    }

    public void Run(Vector2 direction) //ИНКАПУСЛЯЦИЯ
    {
        Vector3 directionCalibrated = new Vector3(direction.x, 0f, direction.y);
        Vector3 nextPosition = _lastPosition += directionCalibrated * _runningSpeed * Time.deltaTime;

        Runned.Invoke(nextPosition);
    }
}
