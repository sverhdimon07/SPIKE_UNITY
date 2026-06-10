using System.IO;
using UnityEngine;

public class JsonRepository<T> : IRepository<T> where T : class
{
    private readonly string _filePath;

    public JsonRepository(string fileName)
    {
        _filePath = Path.Combine(Application.persistentDataPath, fileName);
    }

    public void Save(T data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_filePath, json);
    }

    public T Load()
    {
        if (!File.Exists(_filePath)) return null;
        string json = File.ReadAllText(_filePath);
        return JsonUtility.FromJson<T>(json);
    }
}
