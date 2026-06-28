using System.IO;
using UnityEngine;

public class JsonPlayerRepository : IPlayerRepository
{
    private readonly string _filePath;

    public JsonPlayerRepository(string fileName)
    {
        _filePath = Path.Combine(Application.persistentDataPath, fileName);
    }

    public void Save(PlayerSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_filePath, json);
    }

    public PlayerSaveData Load()
    {
        if (!File.Exists(_filePath)) return null;
        string json = File.ReadAllText(_filePath);
        return JsonUtility.FromJson<PlayerSaveData>(json);
    }
}
