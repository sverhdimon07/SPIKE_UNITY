using UnityEngine;
using UnityEngine.SceneManagement;
//посмотреть, какой контракт создает статический класс - по идее то, что все его внутренние элементы были статическими (НО КОНСТАНТЫ НЕТ, ибо они не могут быть статическими)
public static class SceneLoading //сделать синглтон-сервис (первая причина этому - тут нет метода инита (я добавил срочно надо потому что) И это не сущность, это чисто функциональный класс); НАВЕРНОЕ СТОИТ ПЕРЕИМЕНОВАТЬ В SceneLoading;
{
    private const int MAIN_MENU_SCENE_INDEX = 0;
    private const int TEST_SCENE_INDEX = 1;

    //private GameObject[] _migratingBetweenSceneObjects = new GameObject[1]; //над названием подумать

    private static GameObject _migratingBetweenSceneObject;

    public static void Initialize(GameObject obj)
    {
        _migratingBetweenSceneObject = obj;
    }

    public static void LoadMainMenuScene()
    {
        SceneManager.LoadScene(MAIN_MENU_SCENE_INDEX);
        Time.timeScale = 1f;

        /*
        foreach (GameObject obj in _migratingBetweenSceneObjects)
        {
            SceneManager.MoveGameObjectToScene(obj, SceneManager.GetSceneByBuildIndex(MAIN_MENU_SCENE_INDEX));
        }*/

        //SceneManager.MoveGameObjectToScene(_migratingBetweenSceneObject, SceneManager.GetSceneByBuildIndex(MAIN_MENU_SCENE_INDEX));
    }

    public static void LoadTestScene()
    {
        SceneManager.LoadScene(TEST_SCENE_INDEX);
        Time.timeScale = 1f;

        /*
        foreach (GameObject obj in _migratingBetweenSceneObjects) //и там, и там переношу одинаковые объекты (просто набюдение, мб так и надо в моем случае)
        {
            SceneManager.MoveGameObjectToScene(obj, SceneManager.GetSceneByBuildIndex(TEST_SCENE_INDEX));
        }*/

        //SceneManager.MoveGameObjectToScene(_migratingBetweenSceneObject, SceneManager.GetSceneByBuildIndex(TEST_SCENE_INDEX));
    }

    public static GameObject GetMigratingBetweenSceneObject()
    {
        return _migratingBetweenSceneObject;
    }
}
