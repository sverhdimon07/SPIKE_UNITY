using UnityEngine;
using UnityEngine.Events;

public sealed class PlayerLocomotion
{
    private readonly float _locomotionSpeed;
    private readonly float _runningSpeed;

    private Vector3 _lastPosition;

    public PlayerLocomotion(float locomotionSpeed, float runningSpeed, Vector3 lastPosition)
    {
        _locomotionSpeed = locomotionSpeed;
        _runningSpeed = runningSpeed;
        _lastPosition = lastPosition;
    }

    public UnityAction<Vector3> Locomoted;

    public void Locomote(Vector3 direction) //ИНКАПУСЛЯЦИЯ
    {
        Vector3 nextPosition = _lastPosition += direction * _locomotionSpeed * Time.deltaTime;

        Locomoted.Invoke(nextPosition);
    }

    public void Run(Vector3 direction) //ИНКАПУСЛЯЦИЯ
    {
        Vector3 nextPosition = _lastPosition += direction * _runningSpeed * Time.deltaTime;

        Locomoted.Invoke(nextPosition);
    }
}
