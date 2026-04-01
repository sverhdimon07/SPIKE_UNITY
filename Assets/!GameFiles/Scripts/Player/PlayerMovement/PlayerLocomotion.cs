using UnityEngine;

public sealed class PlayerLocomotion
{
    private float _locomotionSpeed;
    private float _runningSpeed;

    public void Initialize(float locomotionSpeed, float runningSpeed)
    {
        _locomotionSpeed = locomotionSpeed;
        _runningSpeed = runningSpeed;
    }

    public void LocomoteWithinFrame(Transform point, Vector3 requiredActualDirection)
    {
        point.position += requiredActualDirection * _locomotionSpeed * Time.deltaTime;
    }

    public void RunWithinFrame(Transform point, Vector3 requiredActualDirection)
    {
        point.position += requiredActualDirection * _runningSpeed * Time.deltaTime;
    }
}
