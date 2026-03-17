using UnityEngine;

public class EnvironmentAreaOverlapAnalyzerDamageable : EnvironmentAreaOverlapAnalyzer<IDamageable>
{
    public override IDamageable Analyze(Vector3 actingEntityPosition, Vector2 actingEntityDirection)
    {
        return base.Analyze(actingEntityPosition, actingEntityDirection);
    }
}
