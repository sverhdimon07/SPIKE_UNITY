using UnityEngine;
using UnityEngine.Events;

public sealed class PlayerLocomotion
{
    public static UnityAction<Vector3> Locomoted;
    public static UnityAction<Vector3> Runned;

    private readonly float _locomotionSpeed;
    private readonly float _runningSpeed;

    private Vector3 _lastPosition;

    private readonly IEnvironmentAreaAnalyzer<Collider, PlayerController> _environmentAreaAnalyzer;

    public PlayerLocomotion(IEnvironmentAreaAnalyzer<Collider, PlayerController> environmentAreaAnalyzer, Vector2 skeletonAndRenderDirection, float environmentAreaAnalyzerToolDistanceToPlayer, float environmentAreaAnalyzerToolLength, float environmentAreaAnalyzerToolHeight, float locomotionSpeed, float runningSpeed, Vector3 lastPosition)
    {
        _environmentAreaAnalyzer = environmentAreaAnalyzer;
        _environmentAreaAnalyzer.Initialize(_lastPosition, skeletonAndRenderDirection, environmentAreaAnalyzerToolDistanceToPlayer, environmentAreaAnalyzerToolLength, environmentAreaAnalyzerToolHeight);

        _locomotionSpeed = locomotionSpeed;
        _runningSpeed = runningSpeed;
        _lastPosition = lastPosition;
    }

    public void SetLastPosition(Vector3 lastPosition)
    {
        _lastPosition = lastPosition;
    }

    public void Locomote(Vector3 direction, Vector2 skeletonAndRenderDirection) //ИНКАПУСЛЯЦИЯ
    {
        Collider obstacle = _environmentAreaAnalyzer.Analyze(_lastPosition, skeletonAndRenderDirection);

        if ((obstacle != null) && (obstacle.isTrigger == false))
        {
            return;
        }

        Vector3 nextPosition = _lastPosition += direction * _locomotionSpeed * Time.deltaTime;

        Locomoted.Invoke(nextPosition);
    }

    public void Run(Vector3 direction, Vector2 skeletonAndRenderDirection) //ИНКАПУСЛЯЦИЯ
    {
        Collider obstacle = _environmentAreaAnalyzer.Analyze(_lastPosition, skeletonAndRenderDirection);

        if ((obstacle != null) && (obstacle.isTrigger == false))
        {
            return;
        }

        Vector3 nextPosition = _lastPosition += direction * _runningSpeed * Time.deltaTime;

        Runned.Invoke(nextPosition);
    }
}
