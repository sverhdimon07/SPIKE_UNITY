using UnityEngine;

public class EnvironmentAreaOverlapAnalyzerRender : MonoBehaviour
{
    private Vector3 _startPosition;

    private float _raduis;

    private void OnEnable()
    {
        EnvironmentAreaOverlapAnalyzerDamageable.overlapCreated += Initialize; //прям совсем хардкод, работает с конкретной реализацией
    }

    private void OnDisable()
    {
        EnvironmentAreaOverlapAnalyzerDamageable.overlapCreated -= Initialize;
    }

    private void Initialize(Vector3 startPosition, float raduis)
    {
        _startPosition = startPosition;
        _raduis = raduis;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        TryDrawGizmos(_startPosition, _raduis);
    }

    private void TryDrawGizmos(Vector3 _startPosition, float raduis)
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawSphere(_startPosition, _raduis);
    }
#endif
}
