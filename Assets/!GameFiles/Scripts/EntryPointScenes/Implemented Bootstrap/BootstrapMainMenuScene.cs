using UnityEngine;

public sealed class BootstrapMainMenuScene : Bootstrap
{
    [SerializeField] private GameObject _soundDesignObject; //название

    //private GameObject[] _migratingBetweenSceneObjects = new GameObject[1]; //название

    //private SceneLoading _sceneLoading;

    private AudioSource _musicLayerAudioSource; //ПОКА БЕЗ ВЫДЕЛЕННОГО ДЛЯ ЭТОЙ ЗАДАЧИ СЕРВИСА (ПЕРЕДАЛАТЬ)

    private MainMenuUI _ui;

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        _ui.PlayButton.onClick.AddListener(LoadTestScene);
        _ui.SettingsButton.onClick.AddListener(_ui.OpenSettingsWindowCanvas);
        _ui.SettingsExitButton.onClick.AddListener(_ui.CloseSettingsWindowCanvas); //пока так, ибо нужно будет переделать архитектуру под правильную объектную модель
        _ui.MusicVolumeSlider.onValueChanged.AddListener(delegate (float value) { RefreshMusicLayerAudioSourceVolume(_ui.MusicVolumeSlider.value); });
    }

    private void OnDisable()
    {
        _ui.PlayButton.onClick.RemoveListener(LoadTestScene);
        _ui.SettingsButton.onClick.RemoveListener(_ui.OpenSettingsWindowCanvas);
        _ui.SettingsExitButton.onClick.RemoveListener(_ui.CloseSettingsWindowCanvas);
        _ui.MusicVolumeSlider.onValueChanged.RemoveListener(delegate (float value) { RefreshMusicLayerAudioSourceVolume(_ui.MusicVolumeSlider.value); });
    }

    public override void Initialize()
    {
        //_migratingBetweenSceneObjects[0] = _soundDesignObject;
        //_sceneLoading = new SceneLoading();
        _musicLayerAudioSource = FindAnyObjectByType<AudioSource>();
        _ui = FindAnyObjectByType<MainMenuUI>();

        SceneLoading.Initialize(_soundDesignObject); // НЕ СОЗДАЮ В ЭТОЙ СЦЕНЕ МИГРИРУЮЩИЕ ОБЪЕКТЫ
        _ui.Initialize(0.1f); //пока вместо полноценного инита через сервис (я пиал это к тому, когда у нас слайдер детерминировался тем, что стоит в инспекторе) - НАДО ПОНЯТЬ, КТО КОГО ДЕТЕРМИНИРУЕТ
    }

    private void InstantiateMigratingBetweenSceneObjects()
    {
        /*
        foreach (GameObject obj in _sceneLoading.GetMigratingBetweenSceneObjects())
        {
            Instantiate(obj);
        }*/
        //Instantiate(_sceneLoading.GetMigratingBetweenSceneObject());
    }

    private void LoadTestScene()
    {
        SceneLoading.LoadTestScene();
    }

    private void RefreshMusicLayerAudioSourceVolume(float volume)
    {
        _musicLayerAudioSource.volume = volume;
    }
}
