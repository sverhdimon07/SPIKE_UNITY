using Unity.Entities;
using UnityEngine;

class EnemyPrefabHolderAuthoring : MonoBehaviour
{
    public GameObject EnemyPrefab;
}

class EnemyPrefabHolderAuthoringBaker : Baker<EnemyPrefabHolderAuthoring>
{
    public override void Bake(EnemyPrefabHolderAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);

        Entity prefabEntity = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic);

        AddComponent(entity, new EnemyPrefabHolder
        {
            Prefab = prefabEntity
        });
    }
}

public struct EnemyPrefabHolder : IComponentData
{
    public Entity Prefab;
}
