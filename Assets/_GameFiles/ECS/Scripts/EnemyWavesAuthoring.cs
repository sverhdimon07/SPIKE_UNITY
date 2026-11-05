using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class EnemyWavesAuthoring : MonoBehaviour
{
    public List<EnemyWaves> _waves = new List<EnemyWaves>
    {
        new EnemyWaves { WaitTimeBeforeWave = 0f, Amount = 5 },
        new EnemyWaves { WaitTimeBeforeWave = 5f, Amount = 6 },
        new EnemyWaves { WaitTimeBeforeWave = 5f, Amount = 8 },
        new EnemyWaves { WaitTimeBeforeWave = 5f, Amount = 10 },
        new EnemyWaves { WaitTimeBeforeWave = 5f, Amount = 12 }
    };
}

public class EnemyWavesAuthoringBaker : Baker<EnemyWavesAuthoring>
{
    public override void Bake(EnemyWavesAuthoring authoring)
    {
        BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);

        ref WavesBlob root = ref blobBuilder.ConstructRoot<WavesBlob>();

        BlobBuilderArray<Wave> wavesBuilderArray = blobBuilder.Allocate(ref root.Waves, authoring._waves.Count);

        for (int i = 0; i < authoring._waves.Count; i++)
        {
            EnemyWaves authoringWave = authoring._waves[i];

            wavesBuilderArray[i] = new Wave
            {
                WaitTimeBeforeWave = Mathf.Max(authoringWave.WaitTimeBeforeWave, 0f), 
                Amount = Mathf.Max(authoringWave.Amount, 0)
            };
        }

        BlobAssetReference<WavesBlob> wavesBlobAssetReference = blobBuilder.CreateBlobAssetReference<WavesBlob>(Allocator.Persistent);

        blobBuilder.Dispose();

        AddBlobAsset(ref wavesBlobAssetReference, out _);

        Entity entity = GetEntity(TransformUsageFlags.None);

        AddComponent(entity, new EnemyWavesHolder
        {
            WavesReference = wavesBlobAssetReference,
            CurrentWaveIndex = 0,
            Timer = 0f
        });
    }
}

[System.Serializable]
public class EnemyWaves
{
    [Min(0f)]
    public float WaitTimeBeforeWave;

    [Min(0)]
    public int Amount = 5;
}

public struct WavesBlob
{
    public BlobArray<Wave> Waves;
}

public struct Wave
{
    public float WaitTimeBeforeWave;
    public int Amount;
}

public struct EnemyWavesHolder : IComponentData
{
    public BlobAssetReference<WavesBlob> WavesReference;
    public int CurrentWaveIndex;
    public float Timer;
}
