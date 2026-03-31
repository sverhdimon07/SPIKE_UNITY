using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneLoading //сделать синглтон-сервис (первая причина этому - тут нет метода инита И это не сущность, это чисто функциональный класс); НАВЕРНОЕ СТОИТ ПЕРЕИМЕНОВАТЬ В SceneLoading;
{
    private const int MAIN_MENU_SCENE_INDEX = 0;
    private const int TEST_SCENE_INDEX = 1;

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(MAIN_MENU_SCENE_INDEX);
        Time.timeScale = 1f;
    }

    public void LoadTestScene()
    {
        SceneManager.LoadScene(TEST_SCENE_INDEX);
        Time.timeScale = 1f;
    }
}
