using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SavingLoadingJSONRepository<P, R> : ISavingLoadingRepository<P, R> where P : struct where R : struct
{
    private string _filePath;

    public void Initialize(string filePath) //хз, делать ли контракт на инит - ТОЧНО ДА
    {
        _filePath = filePath;
    }

    public void SaveData(P firstData, R secondData)
    {
        string firstJSONFile = JsonUtility.ToJson(firstData);
        string secondJSONFile = JsonUtility.ToJson(secondData);
        Debug.Log(firstJSONFile);
        Debug.Log(secondJSONFile);
        File.WriteAllText(_filePath, firstJSONFile);
        File.AppendAllText(_filePath, "\n===\n");
        File.AppendAllText(_filePath, secondJSONFile);
    }

    public (P, R) LoadData() // хз, делать ли в одну строчку - по идее, это ваще не предметно (ибо это дольше понимать нужно) - а когда у нас логика разделена и причем инкапсулирована в разных методах, это уже предметно
    {
        string jsonFile = File.ReadAllText(_filePath); //хз, есть ли проверка на существование файла
        string separator = "\n===\n";
        var allData = jsonFile.Split(new[] { separator }, System.StringSplitOptions.None);
        string firstJSONFile = allData[0];
        string secondJSONFile = allData[1];

        P a = JsonUtility.FromJson<P>(firstJSONFile);
        R b = JsonUtility.FromJson<R>(secondJSONFile);
        return (a, b);
    }
}
