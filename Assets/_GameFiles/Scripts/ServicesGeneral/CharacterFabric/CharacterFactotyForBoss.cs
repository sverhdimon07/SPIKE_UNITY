using UnityEngine;

public class CharacterFactotyForBoss : MonoBehaviour, ICharacterControllerNewFactory
{
    [SerializeField] private CharacterControllerNewType _typeToSpawn = CharacterControllerNewType.CloseRange;
    [SerializeField] private int _spawnCount = 1;
    [SerializeField] private Transform[] _spawnPositions;
    [SerializeField] private bool _randomPosition = false;
    [SerializeField] private float _randomRadius = 5f;

    [Header("Prefabs")]
    [SerializeField] private CharacterControllerNewCloseRange _closeRangePrefab;
    [SerializeField] private CharacterControllerNewLongRange _longRangePrefab;
    [SerializeField] private CharacterControllerNewBoss _bossPrefab;
    private void Awake()
    {
        PlayerUI.BossSpawned += Spawn;
    }

    public void Spawn()
    {
        for (int i = 0; i < _spawnCount; i++)
        {
            Vector3 pos = GetSpawnPosition(i);
            Create(pos);
        }
    }

    public CharacterControllerNew Create(Vector3 position)
    {
        CharacterControllerNew prefab = GetPrefabByType(_typeToSpawn);
        if (prefab == null)
        {
            return null;
        }
        return Instantiate(prefab, position, Quaternion.identity);
    }

    private Vector3 GetSpawnPosition(int index)
    {
        if (_spawnPositions != null && index < _spawnPositions.Length)
            return _spawnPositions[index].position;

        if (_randomPosition)
            return transform.position + Random.insideUnitSphere * _randomRadius;

        return transform.position;
    }

    private CharacterControllerNew GetPrefabByType(CharacterControllerNewType type)
    {
        switch (type)
        {
            case CharacterControllerNewType.CloseRange: return _closeRangePrefab;
            case CharacterControllerNewType.LongRange: return _longRangePrefab;
            case CharacterControllerNewType.Boss: return _bossPrefab;
            default: return null;
        }
    }
}
