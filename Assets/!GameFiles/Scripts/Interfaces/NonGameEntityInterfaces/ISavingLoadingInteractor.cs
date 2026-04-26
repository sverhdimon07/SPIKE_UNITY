using UnityEngine;

public interface ISavingLoadingInteractor
{
    public void Initialize(ISavingLoadingRepository<Vector3, Quaternion> savingLoadingRepository, string filePath, Player player);

    public void SaveData(Player player); //НУЖНО сделать обощенный тип (было взодное значение MonoBehaviour entityMono)

    public void LoadData(Player player);
}
