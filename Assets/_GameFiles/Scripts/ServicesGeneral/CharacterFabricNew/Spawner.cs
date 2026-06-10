using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    [Header("Enemy Types")]
    [SerializeField] private List<GameObject> characterPrefabs;

    [Header("Spawn Settings")]
    [SerializeField] private int totalEnemies = 5;   
    [SerializeField] private float spawnRadius = 20f; 
    [SerializeField] private float minDistanceBetween = 3f;

    [Header("Alternative Area (rect)")]
    [SerializeField] private bool useAreaBounds = false; 
    [SerializeField] private Vector3 areaMin = new Vector3(-30, 0, -30);
    [SerializeField] private Vector3 areaMax = new Vector3(30, 0, 30);

    private void Start()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        if (characterPrefabs == null || characterPrefabs.Count == 0)
        {
            Debug.LogError("Spawner: no enemy prefabs assigned!");
            return;
        }

        List<Vector3> usedPositions = new List<Vector3>();

        for (int i = 0; i < totalEnemies; i++)
        {
            // случайный выбор типа врага из списка
            GameObject prefab = characterPrefabs[Random.Range(0, characterPrefabs.Count)];
            Vector3 spawnPos = GetValidSpawnPosition(usedPositions);

            if (spawnPos != Vector3.zero)
            {
                Instantiate(prefab, spawnPos, Quaternion.identity);
                usedPositions.Add(spawnPos);
            }
            else
            {
                Debug.LogWarning($"Spawner: failed to find position for enemy {i}");
            }
        }
    }

    private Vector3 GetValidSpawnPosition(List<Vector3> existingPositions)
    {
        int attempts = 30;
        for (int i = 0; i < attempts; i++)
        {
            // получаем кандидат в зависимости от режима
            Vector3 candidate;
            if (useAreaBounds)
            {
                float x = Random.Range(areaMin.x, areaMax.x);
                float z = Random.Range(areaMin.z, areaMax.z);
                candidate = new Vector3(x, 0, z);
            }
            else
            {
                Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
                candidate = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
            }

            // ищем ближайшую точку на NavMesh
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                Vector3 finalPos = hit.position;

                // проверяем расстояние до уже созданных врагов
                bool tooClose = false;
                foreach (var pos in existingPositions)
                {
                    if (Vector3.Distance(finalPos, pos) < minDistanceBetween)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (!tooClose)
                    return finalPos;
            }
        }
        return Vector3.zero;
    }

    // визуализация зоны спавна в редакторе
    private void OnDrawGizmosSelected()
    {
        if (useAreaBounds)
        {
            Gizmos.color = Color.green;
            Vector3 center = (areaMin + areaMax) / 2;
            Vector3 size = areaMax - areaMin;
            Gizmos.DrawWireCube(center, size);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}
