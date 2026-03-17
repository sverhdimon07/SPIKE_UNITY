using UnityEngine;

public class EnvironmentAreaRaycastAnalyzer<T> : IEnvironmentAreaAnalyzer<T> where T : class
{
    public void Initialize(Vector3 actingEntityPosition, Vector2 actingEntityDirection, float range)
    {
        //
    }

    public T Analyze(Vector3 actingEntityPosition, Vector2 actingEntityDirection)
    {
        return null;
        //
    }
}
