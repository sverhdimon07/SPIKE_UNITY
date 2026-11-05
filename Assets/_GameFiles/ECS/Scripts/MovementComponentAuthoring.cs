using Unity.Entities;
using UnityEngine;

public class MovementComponentAuthoring : MonoBehaviour
{
    [Min(0f)]
    public float Speed = 2f;
}

public class MovementComponentAuthoringBaker : Baker<MovementComponentAuthoring>
{
    public override void Bake(MovementComponentAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new MovementComponent
        {
            Speed = authoring.Speed,
            IsMoving = false
        });
    }
}

public struct MovementComponent : IComponentData
{
    public float Speed;

    public bool IsMoving;
}
