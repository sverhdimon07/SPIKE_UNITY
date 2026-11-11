using UnityEngine;
using UnityEngine.Events;

public sealed class PlayerLocomotion
{
    public static UnityAction<Vector3> Locomoted;
    public static UnityAction<Vector3> Runned;

    public event UnityAction<FlightLimiter> FlightLimiterReached;
    public event UnityAction<Vector3> AirborneMoved;

    private const float CollisionSkin = 0.02f;
    private const float FallAcceleration = 18f;
    private const float MaxFallSpeed = 25f;

    private readonly CapsuleCollider _capsule;
    private readonly float _flightSpeed;
    private readonly RaycastHit[] _airborneHits = new RaycastHit[8];

    public bool IsFalling { get; private set; }
    public float FallSpeed { get; private set; }

    private readonly float _locomotionSpeed;
    private readonly float _runningSpeed;

    private Vector3 _lastPosition;

    private readonly IEnvironmentAreaAnalyzer<Collider, PlayerController> _environmentAreaAnalyzer;

    public PlayerLocomotion(IEnvironmentAreaAnalyzer<Collider, PlayerController> environmentAreaAnalyzer, Vector2 skeletonAndRenderDirection, float environmentAreaAnalyzerToolDistanceToPlayer, float environmentAreaAnalyzerToolLength, float environmentAreaAnalyzerToolHeight, float locomotionSpeed, float runningSpeed, Vector3 lastPosition, CapsuleCollider capsule, float flightSpeed)
    {
        _capsule = capsule;
        _flightSpeed = flightSpeed;
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

    public bool Fly(Vector3 direction)
    {
        float distance = _flightSpeed * Time.deltaTime;
        if (distance <= 0f || direction == Vector3.zero)
        {
            return false;
        }

        ScanAirbornePath(direction, distance + CollisionSkin, QueryTriggerInteraction.Collide,
            out float obstacleDistance, out float limiterDistance, out FlightLimiter limiter);
        if (limiter != null && limiterDistance <= distance && limiterDistance <= obstacleDistance)
        {
            MoveInAir(direction * limiterDistance);
            FlightLimiterReached?.Invoke(limiter);
            return false;
        }

        if (obstacleDistance < float.PositiveInfinity)
        {
            return false;
        }

        MoveInAir(direction * distance);
        return true;
    }

    public void RefreshGroundSupport(float fallSpeed = 0f)
    {
        if (TryGetAirborneObstacle(Vector3.down, 2f * CollisionSkin, out _))
        {
            ClearFall();
            return;
        }

        IsFalling = true;
        FallSpeed = Mathf.Clamp(fallSpeed, 0f, MaxFallSpeed);
    }

    public void ClearFall()
    {
        IsFalling = false;
        FallSpeed = 0f;
    }

    public void UpdateGravity()
    {
        float deltaTime = Time.deltaTime;
        if (deltaTime <= 0f)
        {
            return;
        }

        float fallSpeed = Mathf.Min(FallSpeed + FallAcceleration * deltaTime, MaxFallSpeed);
        float distance = fallSpeed * deltaTime;
        float probeDistance = Mathf.Max(distance, CollisionSkin) + CollisionSkin;
        if (TryGetAirborneObstacle(Vector3.down, probeDistance, out float hitDistance))
        {
            ClearFall();
            distance = Mathf.Max(0f, hitDistance - CollisionSkin);
        }
        else
        {
            IsFalling = true;
            FallSpeed = fallSpeed;
        }

        if (distance > 0f)
        {
            MoveInAir(Vector3.down * distance);
        }
    }

    private void MoveInAir(Vector3 displacement)
    {
        _lastPosition += displacement;
        AirborneMoved?.Invoke(_lastPosition);
    }

    private bool TryGetAirborneObstacle(Vector3 direction, float distance, out float hitDistance)
    {
        ScanAirbornePath(direction, distance, QueryTriggerInteraction.Ignore, out hitDistance, out _, out _);
        return hitDistance < float.PositiveInfinity;
    }

    private void ScanAirbornePath(Vector3 direction, float distance, QueryTriggerInteraction triggers,
        out float obstacleDistance, out float limiterDistance, out FlightLimiter closestLimiter)
    {
        Transform capsuleTransform = _capsule.transform;
        Vector3 scale = capsuleTransform.lossyScale;
        float radius = _capsule.radius * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));
        float halfSegment = Mathf.Max(0f, _capsule.height * Mathf.Abs(scale.y) * 0.5f - radius);
        Vector3 center = capsuleTransform.TransformPoint(_capsule.center);
        Vector3 axis = capsuleTransform.up * halfSegment;
        float castRadius = Mathf.Max(radius - CollisionSkin, radius * 0.5f);

        RaycastHit[] hits = _airborneHits;
        int count = Physics.CapsuleCastNonAlloc(center - axis, center + axis, castRadius,
            direction, hits, distance, Physics.DefaultRaycastLayers, triggers);
        if (count == hits.Length)
        {
            hits = Physics.CapsuleCastAll(center - axis, center + axis, castRadius,
                direction, distance, Physics.DefaultRaycastLayers, triggers);
            count = hits.Length;
        }

        obstacleDistance = float.PositiveInfinity;
        limiterDistance = float.PositiveInfinity;
        closestLimiter = null;
        for (int i = 0; i < count; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.collider.transform.IsChildOf(capsuleTransform))
            {
                continue;
            }

            if (hit.collider.isTrigger)
            {
                if (hit.distance < limiterDistance && hit.collider.TryGetComponent(out FlightLimiter limiter) && limiter.isActiveAndEnabled)
                {
                    limiterDistance = hit.distance;
                    closestLimiter = limiter;
                }
            }
            else if (Vector3.Dot(hit.normal, direction) < -0.001f)
            {
                obstacleDistance = Mathf.Min(obstacleDistance, hit.distance);
            }
        }
    }
}
