using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(StaminaSystem))]
[BurstCompile]
public partial struct EnemyDestroySystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton(out TargetComponent target))
        {
            return;
        }

        float destroyDistance = 0.15f;
        float destroyDistanceSq = destroyDistance * destroyDistance;

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (transform, entity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<MovementComponent, StaminaComponent>().WithEntityAccess())
        {
            float distanceSq = math.distancesq(transform.ValueRO.Position, target.Position);

            if (distanceSq <= destroyDistanceSq)
            {
                ecb.DestroyEntity(entity);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
