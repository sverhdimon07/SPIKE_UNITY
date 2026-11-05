using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(EnemySpawnSystem))]
[BurstCompile]
public partial struct MovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton<TargetComponent>(out TargetComponent target))
        {
            return;
        }

        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (transform, movement, stamina) in
                 SystemAPI.Query<
                     RefRW<LocalTransform>,
                     RefRW<MovementComponent>,
                     RefRO<StaminaComponent>>())
        {
            if (stamina.ValueRO.IsRecovering ||
                stamina.ValueRO.Current <= 0f ||
                movement.ValueRO.Speed <= 0f)
            {
                movement.ValueRW.IsMoving = false;
                continue;
            }

            float3 direction = target.Position - transform.ValueRO.Position;
            float distance = math.length(direction);

            if (distance <= 0.05f)
            {
                transform.ValueRW.Position = target.Position;
                movement.ValueRW.IsMoving = false;
                continue;
            }

            float moveDistance = movement.ValueRO.Speed * deltaTime;

            if (distance <= moveDistance)
            {
                transform.ValueRW.Position = target.Position;
                movement.ValueRW.IsMoving = false;
                continue;
            }

            float3 normalizedDirection = math.normalizesafe(direction);

            transform.ValueRW.Position += normalizedDirection * moveDistance;
            movement.ValueRW.IsMoving = true;
        }
    }
}
