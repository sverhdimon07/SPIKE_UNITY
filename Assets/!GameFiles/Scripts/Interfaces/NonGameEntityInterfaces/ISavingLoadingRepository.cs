public interface ISavingLoadingRepository
{
    public void Initialize(string filePath);

    public void SaveData(object data);

    public object LoadData();
}
