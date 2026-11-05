using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[UpdateAfter(typeof(MovementSystem))]
[BurstCompile]
public partial struct StaminaSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (stamina, movement) in
                 SystemAPI.Query<
                     RefRW<StaminaComponent>,
                     RefRO<MovementComponent>>())
        {
            if (stamina.ValueRO.IsRecovering)
            {
                float current = math.min(
                    stamina.ValueRO.Current +
                    stamina.ValueRO.RecoveryPerSecond * deltaTime,
                    stamina.ValueRO.Max);

                stamina.ValueRW.Current = current;

                if (current >= stamina.ValueRO.Max)
                {
                    stamina.ValueRW.IsRecovering = false;
                }

                continue;
            }

            if (!movement.ValueRO.IsMoving)
            {
                continue;
            }

            float remaining = math.max(
                stamina.ValueRO.Current -
                stamina.ValueRO.DrainPerSecond * deltaTime,
                0f);

            stamina.ValueRW.Current = remaining;

            if (remaining <= 0f)
            {
                stamina.ValueRW.IsRecovering = true;
            }
        }
    }
}
