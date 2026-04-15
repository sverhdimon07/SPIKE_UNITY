using System.IO;
using UnityEngine;

public class SavingLoadingJSONRepository : ISavingLoadingRepository
{
    private string _filePath;

    public void Initialize(string filePath) //хз, делать ли контракт на инит - ТОЧНО ДА
    {
        _filePath = filePath;
    }

    public void SaveData(object data)
    {
        string jsonFile = JsonUtility.ToJson(data);

        File.WriteAllText(_filePath, jsonFile);
    }

    public object LoadData()
    {
        string jsonFile = File.ReadAllText(_filePath); //хз, есть ли проверка на существование файла

        return JsonUtility.FromJson<object>(jsonFile); // хз, делать ли в одну строчку - по идее, это ваще не предметно (ибо это дольше понимать нужно) - а когда у нас логика разделена и причем инкапсулирована в разных методах, это уже предметно
    }
}
