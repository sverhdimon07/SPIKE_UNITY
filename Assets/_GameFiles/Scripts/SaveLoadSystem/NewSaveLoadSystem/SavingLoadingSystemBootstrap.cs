using UnityEngine;
using System.Collections.Generic;

public class SavingLoadingSystemBootstrap : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private PlayerController _playerController;
    // Больше не нужен одиночный _characterController — враги будут найдены автоматически.

    private Interactor<PlayerSavingData> _playerInteractor;
    private Interactor<EnemiesSavingData> _enemiesInteractor;

    private const string PLAYER_FILE = "player_save.json";
    private const string ENEMIES_FILE = "enemies_save.json";

    private void Awake()
    {
        var playerRepo = new JsonRepository<PlayerSavingData>(PLAYER_FILE);
        var enemiesRepo = new JsonRepository<EnemiesSavingData>(ENEMIES_FILE);

        _playerInteractor = new Interactor<PlayerSavingData>(playerRepo);
        _enemiesInteractor = new Interactor<EnemiesSavingData>(enemiesRepo);

        LoadAll();

        Application.quitting += OnApplicationQuitting;
    }

    private void OnDestroy()
    {
        Application.quitting -= OnApplicationQuitting;
    }

    private void OnApplicationQuitting()
    {
        SaveAll();
    }

    public void SaveAll()
    {
        SavePlayer();
        SaveEnemies();  // Сохраняем всех врагов
    }

    public void LoadAll()
    {
        LoadPlayer();
        LoadEnemies();  // Загружаем всех врагов
    }

    // === ИГРОК (без изменений) ===
    private void SavePlayer()
    {
        if (_playerController == null)
        {
            Debug.LogError("PlayerController missing");
            return;
        }

        var data = new PlayerSavingData
        {
            position = _playerController.transform.position,
            rotation = _playerController.gameObject.transform.rotation,
        };

        _playerInteractor.Save(data);
        Debug.Log("Player saved");
    }

    private void LoadPlayer()
    {
        var data = _playerInteractor.Load();
        if (data != null)
        {
            _playerController.transform.position = data.position;
            _playerController.SetRenderAndSkeletonPivot(data.rotation);
            Debug.Log("Player loaded");
        }
        else
        {
            Debug.Log("No player save file found");
        }
    }

    // === НОВОЕ: сохранение и загрузка всех врагов ===
    private void SaveEnemies()
    {
        // Находим всех врагов на сцене по компоненту (замените на ваш класс врага)
        CharacterControllerNew[] allEnemies = FindObjectsOfType<CharacterControllerNew>();

        if (allEnemies.Length == 0)
        {
            Debug.Log("No enemies found to save");
            return;
        }

        List<EnemySaveData> enemiesDataList = new List<EnemySaveData>();

        foreach (var enemy in allEnemies)
        {
            enemiesDataList.Add(new EnemySaveData
            {
                position = enemy.transform.position,
                rotation = enemy.transform.rotation
                // При необходимости добавьте другие поля (здоровье, тип и т.д.)
            });
        }

        EnemiesSavingData wrapper = new EnemiesSavingData { enemies = enemiesDataList.ToArray() };
        _enemiesInteractor.Save(wrapper);
        Debug.Log($"Saved {enemiesDataList.Count} enemies");
    }

    private void LoadEnemies()
    {
        EnemiesSavingData wrapper = _enemiesInteractor.Load();
        if (wrapper == null || wrapper.enemies == null)
        {
            Debug.Log("No enemies save file found");
            return;
        }

        // Находим текущих врагов на сцене
        CharacterControllerNew[] currentEnemies = FindObjectsOfType<CharacterControllerNew>();

        // Восстанавливаем состояние каждому врагу
        // Важно: данные сохраняются в том же порядке, что и FindObjectsOfType?
        // Порядок может меняться, поэтому лучше сопоставлять по уникальному ID.
        // Для простоты предположим, что количество врагов и их порядок совпадают.
        // Если враги динамически создавались/удалялись, лучше использовать GUID.
        if (currentEnemies.Length != wrapper.enemies.Length)
        {
            Debug.LogWarning($"Number of enemies changed: saved {wrapper.enemies.Length}, current {currentEnemies.Length}. Restoration may be incorrect.");
        }

        int count = Mathf.Min(currentEnemies.Length, wrapper.enemies.Length);
        for (int i = 0; i < count; i++)
        {
            currentEnemies[i].transform.position = wrapper.enemies[i].position;
            currentEnemies[i].transform.rotation = wrapper.enemies[i].rotation;
        }

        Debug.Log($"Loaded {count} enemies");
    }
}

// ---- КЛАССЫ ДАННЫХ ДЛЯ ВРАГОВ ----

// Данные одного врага
[System.Serializable]
public class EnemySaveData
{
    public Vector3 position;
    public Quaternion rotation;
    // Добавьте другие нужные поля (здоровье, тип, и т.д.)
}

// Контейнер для списка врагов
[System.Serializable]
public class EnemiesSavingData
{
    public EnemySaveData[] enemies;
}