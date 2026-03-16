using UnityEngine;

public class EnvironmentAreaColliderAnalyzer : IEnvironmentAreaAnalyzer
{
    private SphereCollider _collider;

    public void Initialize(Vector3 playerPosition, Vector2 playerDirection, Vector3 weaponPrefabScale, float weaponAttackRange)
    {
        //_collider = collider; //пока что мы не создаем инструмент здесь (я не придумал, как его не создавать каждый раз при атаке), я создал коллайдер на сцене и здесь его прокидываю
    }

    public IDamageable Analyze(Vector3 playerPosition, Vector2 playerDirection)
    {
        return null;
        /*
        if (_collider.TryGetComponent(out IDamageable damageable) == false)
        {
            return null;
        }
        return damageable;*/
    }
}
