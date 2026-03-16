using UnityEngine;

public class EnvironmentAreaOverlapAnalyzerRender : MonoBehaviour
{
    private Vector3 _startPosition;

    private float _raduis;

    private void OnEnable()
    {
        EnvironmentAreaOverlapAnalyzer.overlapCreated += Initialize;
    }

    private void OnDisable()
    {
        EnvironmentAreaOverlapAnalyzer.overlapCreated -= Initialize;
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
