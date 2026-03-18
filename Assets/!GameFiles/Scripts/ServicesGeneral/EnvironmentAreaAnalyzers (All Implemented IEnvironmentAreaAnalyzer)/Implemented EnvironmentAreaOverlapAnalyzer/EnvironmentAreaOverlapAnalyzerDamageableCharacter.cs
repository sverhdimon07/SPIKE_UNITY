using UnityEngine;

public class EnvironmentAreaOverlapAnalyzerDamageableCharacter : EnvironmentAreaOverlapAnalyzer<IDamageableCharacter>
{
    public override IDamageableCharacter Analyze(Vector3 actingEntityPosition, Vector2 actingEntityDirection)
    {
        return base.Analyze(actingEntityPosition, actingEntityDirection);
    }
}
