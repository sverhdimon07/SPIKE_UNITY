using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TargetAuthoring : MonoBehaviour
{
}

public class TargetAuthoringBaker : Baker<TargetAuthoring>
{
    public override void Bake(TargetAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new TargetComponent()
        {
            Position = authoring.transform.position
        });
    }
}

public struct TargetComponent : IComponentData
{
    public float3 Position;
}
