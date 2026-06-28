using System.IO;
using UnityEngine;

public class JsonCharacterRepository : ICharacterRepository
{
    private readonly string _filePath;

    public JsonCharacterRepository(string fileName)
    {
        _filePath = Path.Combine(Application.persistentDataPath, fileName);
    }

    public void Save(SavedCharactersData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_filePath, json);
    }

    public SavedCharactersData Load()
    {
        if (!File.Exists(_filePath)) return null;
        string json = File.ReadAllText(_filePath);
        return JsonUtility.FromJson<SavedCharactersData>(json);
    }
}
