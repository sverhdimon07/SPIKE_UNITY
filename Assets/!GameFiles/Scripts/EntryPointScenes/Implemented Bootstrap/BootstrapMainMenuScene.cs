using UnityEngine;

public sealed class BootstrapMainMenuScene : Bootstrap
{
    private SceneLoading _sceneLoading;

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
        _sceneLoading = new SceneLoading();
        _musicLayerAudioSource = FindAnyObjectByType<AudioSource>();
        _ui = FindAnyObjectByType<MainMenuUI>();

        _ui.Initialize(0.1f); //пока вместо полноценного инита через сервис (я пиал это к тому, когда у нас слайдер детерминировался тем, что стоит в инспекторе) - НАДО ПОНЯТЬ, КТО КОГО ДЕТЕРМИНИРУЕТ
    }

    private void LoadTestScene()
    {
        _sceneLoading.LoadTestScene();
    }

    private void RefreshMusicLayerAudioSourceVolume(float volume)
    {
        _musicLayerAudioSource.volume = volume;
    }
}
