using UnityEngine;

public sealed class EnvironmentAreaOverlapAnalyzerRender : MonoBehaviour
{
    private Vector3 _startSphereCenterPosition;

    private float _sphereRaduis;

    private void OnEnable()
    {
        EnvironmentAreaOverlapAnalyzer<IDamageable, Player>.OverlapCreated += Initialize; //прям совсем хардкод, работает с конкретной реализацией
        EnvironmentAreaOverlapAnalyzer<IDamageable, Character>.OverlapCreated += Initialize; //прям совсем хардкод, работает с конкретной реализацией
    }

    private void OnDisable()
    {
        EnvironmentAreaOverlapAnalyzer<IDamageable, Player>.OverlapCreated -= Initialize;
        EnvironmentAreaOverlapAnalyzer<IDamageable, Character>.OverlapCreated -= Initialize;
    }

    private void Initialize(Vector3 startSphereCenterPosition, float sphereRaduis)
    {
        _startSphereCenterPosition = startSphereCenterPosition;
        _sphereRaduis = sphereRaduis;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        TryDrawGizmos();
    }

    private void TryDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(_startSphereCenterPosition, _sphereRaduis);
    }
#endif
}
