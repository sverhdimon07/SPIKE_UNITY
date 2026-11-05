using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct EnemySpawnSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton(out TargetComponent target))
        {
            return;
        }

        if (!SystemAPI.TryGetSingleton(out EnemyPrefabHolder prefabHolder))
        {
            return;
        }

        if (!SystemAPI.TryGetSingleton(out EnemySpawnerSettings settings))
        {
            return;
        }

        if (!SystemAPI.TryGetSingleton(out EnemyWavesHolder wavesHolder))
        {
            return;
        }

        Entity wavesEntity = SystemAPI.GetSingletonEntity<EnemyWavesHolder>();
        Entity settingsEntity = SystemAPI.GetSingletonEntity<EnemySpawnerSettings>();

        if (!wavesHolder.WavesReference.IsCreated)
        {
            return;
        }

        ref BlobArray<Wave> waves = ref wavesHolder.WavesReference.Value.Waves;

        if (wavesHolder.CurrentWaveIndex >= waves.Length)
        {
            return;
        }

        wavesHolder.Timer += SystemAPI.Time.DeltaTime;

        ref Wave currentWave = ref waves[wavesHolder.CurrentWaveIndex];

        if (wavesHolder.Timer < currentWave.WaitTimeBeforeWave)
        {
            state.EntityManager.SetComponentData(wavesEntity, wavesHolder);
            return;
        }

        wavesHolder.Timer = 0f;
        wavesHolder.CurrentWaveIndex++;

        state.EntityManager.SetComponentData(wavesEntity, wavesHolder);

        int enemyCount = math.max(currentWave.Amount, 0);

        if (enemyCount == 0)
        {
            return;
        }

        StaminaComponent staminaTemplate = state.EntityManager.GetComponentData<StaminaComponent>(prefabHolder.Prefab);

        float minStamina = math.clamp(settings.MinStamina, 0f, staminaTemplate.Max);

        float maxStamina = math.clamp(settings.MaxStamina, minStamina, staminaTemplate.Max);

        float radius = math.max(settings.SpawnRadius, 0.1f);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        for (int i = 0; i < enemyCount; i++)
        {
            float angle = 2f * math.PI * i / enemyCount;

            float3 spawnPosition = target.Position + new float3(
                math.cos(angle) * radius,
                0f,
                math.sin(angle) * radius);

            Entity enemy = ecb.Instantiate(prefabHolder.Prefab);

            ecb.SetComponent(enemy, LocalTransform.FromPosition(spawnPosition));

            StaminaComponent stamina = staminaTemplate;

            if (maxStamina > minStamina)
            {
                stamina.Current = settings.Random.NextFloat(minStamina, maxStamina);
            }
            else
            {
                stamina.Current = minStamina;
            }

            stamina.IsRecovering = false;

            ecb.SetComponent(enemy, stamina);
        }

        state.EntityManager.SetComponentData(settingsEntity, settings);

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
