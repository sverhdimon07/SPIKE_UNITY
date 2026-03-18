using UnityEngine;

public class EnvironmentAreaOverlapAnalyzerDamageablePlayer : EnvironmentAreaOverlapAnalyzer<IDamageablePlayer>
{
    public override IDamageablePlayer Analyze(Vector3 actingEntityPosition, Vector2 actingEntityDirection)
    {
        return base.Analyze(actingEntityPosition, actingEntityDirection);
    }
}
