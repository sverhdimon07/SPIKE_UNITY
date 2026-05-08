using UnityEngine;

public interface ISavingLoadingInteractor
{
    public void Initialize(ISavingLoadingRepository<Vector3, Quaternion> savingLoadingRepository, string filePath, PlayerController playerController);

    public void SaveData(PlayerController playerController); //НУЖНО сделать обощенный тип (было взодное значение MonoBehaviour entityMono)

    public void LoadData(PlayerController playerController);
}
