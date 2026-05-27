using UnityEngine;
using UnityEngine.Events;

public sealed class PlayerLocomotion
{
    public static UnityAction<Vector3> Locomoted;
    public static UnityAction<Vector3> Runned;

    private readonly float _locomotionSpeed;
    private readonly float _runningSpeed;

    private Vector3 _lastPosition;

    public PlayerLocomotion(float locomotionSpeed, float runningSpeed, Vector3 lastPosition)
    {
        _locomotionSpeed = locomotionSpeed;
        _runningSpeed = runningSpeed;
        _lastPosition = lastPosition;
    }

    public void Locomote(Vector3 direction) //ИНКАПУСЛЯЦИЯ
    {
        Vector3 nextPosition = _lastPosition += direction * _locomotionSpeed * Time.deltaTime;
        //Debug.Log(Locomoted.GetInvocationList());
        Locomoted.Invoke(nextPosition);
        //Debug.Log("AAAAAAA");
    }

    public void Run(Vector3 direction) //ИНКАПУСЛЯЦИЯ
    {
        Vector3 nextPosition = _lastPosition += direction * _runningSpeed * Time.deltaTime;

        Runned.Invoke(nextPosition);
    }
}
