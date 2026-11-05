using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class EnemySpawnerSettingsAuthoring : MonoBehaviour
{
    [Min(0.1f)]
    public float SpawnRadius = 8f;

    [Min(0f)]
    public float MinStamina = 30f;

    [Min(0f)]
    public float MaxStamina = 100f;
}

class EnemySpawnerSettingsAuthoringBaker : Baker<EnemySpawnerSettingsAuthoring>
{
    public override void Bake(EnemySpawnerSettingsAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);

        uint seed = (uint)UnityEngine.Random.Range(1, 1000000);

        float minStamina = Mathf.Max(authoring.MinStamina, 0f);
        float maxStamina = Mathf.Max(authoring.MaxStamina, minStamina);

        AddComponent(entity, new EnemySpawnerSettings
        {
            SpawnRadius = authoring.SpawnRadius,
            MinStamina = minStamina,
            MaxStamina = maxStamina,
            Random = new Unity.Mathematics.Random(seed)
        });
    }
}

public struct EnemySpawnerSettings : IComponentData
{
    public float SpawnRadius;
    public float MinStamina;
    public float MaxStamina;
    public Unity.Mathematics.Random Random;
}
