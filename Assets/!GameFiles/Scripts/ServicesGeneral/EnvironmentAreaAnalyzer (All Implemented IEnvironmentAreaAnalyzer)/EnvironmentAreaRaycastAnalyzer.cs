using UnityEngine;

public class EnvironmentAreaRaycastAnalyzer : IEnvironmentAreaAnalyzer
{
    public void Initialize(Vector3 playerPosition, Vector2 playerDirection, Vector3 weaponPrefabScale, float weaponAttackRange)
    {
        //
    }

    public IDamageable Analyze(Vector3 playerPosition, Vector2 playerDirection)
    {
        return null;
        //
    }
}
