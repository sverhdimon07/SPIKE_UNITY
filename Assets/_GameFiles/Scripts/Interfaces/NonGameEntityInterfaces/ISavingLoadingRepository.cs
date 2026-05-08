using System.Collections.Generic;

public interface ISavingLoadingRepository<P, R> where P : struct where R : struct
{
    public void Initialize(string filePath);

    public void SaveData(P firstData, R secondData);

    public (P, R) LoadData();
}
