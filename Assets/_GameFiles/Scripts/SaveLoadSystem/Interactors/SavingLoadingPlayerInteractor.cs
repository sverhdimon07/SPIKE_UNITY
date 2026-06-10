using System.IO;
using UnityEngine;

public sealed class SavingLoadingPlayerInteractor : MonoBehaviour, ISavingLoadingInteractor
{
    private ISavingLoadingRepository<Vector3, Quaternion> _savingLoadingRepository;

    public void Initialize(ISavingLoadingRepository<Vector3, Quaternion> savingLoadingRepository, string filePath, PlayerController playerController) //хз, делать ли контракт на инит - ТОЧНО ДА
    {
        _savingLoadingRepository = savingLoadingRepository;

        _savingLoadingRepository.Initialize(filePath);
        
        if (File.Exists(filePath) == false)
        {
            SaveData(playerController);
        }
        else //я где-то делал ненужные if else, надо переделать их под просто else
        {
            LoadData(playerController); //при первом запуске игры мы должны чекать - есть ли у нас какое-то сохранение. Если нет - ничего загружать не нужно, запускаем сцену с чистого листа и без сохранения. Если да, то запускаем сцену, загружая при этом текущее сохранение;
        }
    }

    public void SaveData(PlayerController playerController)
    {
        //Player player = FindAnyObjectByType<Player>(); //ХАРДКОД
        //Player player = (Player)playerMono; //ДАУНКАСТ - исправить обощенныйм типом
        Vector3 position = playerController.transform.position;
        Quaternion rotation = playerController.gameObject.transform.rotation;
        //List<Vector3, Quaternion> savingObjects = new List<Vector3, Quaternion>();
        //SavingLoadingPlayerData _savingLoadingPlayerData = new SavingLoadingPlayerData();
        //_savingLoadingPlayerData.Position = position;
        //_savingLoadingPlayerData.Rotation = rotation;
        //savingObjects.Add(_savingLoadingPlayerData);

        _savingLoadingRepository.SaveData(position, rotation);
        //print(_savingLoadingPlayerData.Position);
        //print(_savingLoadingPlayerData.Rotation);
    }

    public void LoadData(PlayerController playerController) //первый раз я писал тут в возращающем значении object[]
    {
        //Player player = FindAnyObjectByType<Player>(); //ХАРДКОД
        //Player player = (Player)playerMono; //ДАУНКАСТ - исправить обощенныйм типом
        (Vector3 a, Quaternion b) = _savingLoadingRepository.LoadData(); //статический анализ предложит вызвать метод с ?
        //print(loadingObjects.Count);
        playerController.transform.position = a;
        playerController.SetRenderAndSkeletonPivot(b);
        //print(loadingObjects[0]);
        //print(loadingObjects[1]);
        /*
        foreach (object loadingObject in loadingObjects)
        {
            if (loadingObject.GetType() == typeof(Vector3))
            {
                _player.transform.position = (Vector3)loadingObject;
            }
            else if (loadingObject.GetType() == typeof(Quaternion))
            {
                _player.SetRenderAndSkeletonPoint((Quaternion)loadingObject);
            }
        }*/
    }
}
