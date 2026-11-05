using Unity.Entities;
using UnityEngine;

public class StaminaComponentAuthoring : MonoBehaviour
{
    [Min(1f)]
    public float MaxStamina = 100f;

    [Min(0.01f)]
    public float DrainDuration = 10f;

    [Min(0.01f)]
    public float RecoveryDuration = 5f;
}

public class StaminaComponentAuthoringBaker : Baker<StaminaComponentAuthoring>
{
    public override void Bake(StaminaComponentAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new StaminaComponent
        {
            Current = authoring.MaxStamina,
            Max = authoring.MaxStamina,
            DrainPerSecond = authoring.MaxStamina / authoring.DrainDuration,
            RecoveryPerSecond = authoring.MaxStamina / authoring.RecoveryDuration,
            IsRecovering = false
        });
    }
}

public struct StaminaComponent : IComponentData
{
    public float Current;
    public float Max;
    public float DrainPerSecond;
    public float RecoveryPerSecond;
    public bool IsRecovering;
}
