using UnityEngine;
using UnityEngine.UI;

public sealed class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Canvas _startWindowCanvas;
    [SerializeField] private Canvas _settingsWindowCanvas;

    [SerializeField] private Button _playButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _settingsExitButton; //пока так, ибо нужно будет переделать архитектуру под правильную объектную модель

    [SerializeField] private Slider _musicVolumeSlider;

    public Button PlayButton => _playButton;
    public Button SettingsButton => _settingsButton;
    public Button SettingsExitButton => _settingsExitButton;
    
    public Slider MusicVolumeSlider => _musicVolumeSlider;

    public void Initialize(float sliderValue)
    {
        RefreshSlider(sliderValue); //ВОТ ЭТИ ВСЕ МЕТОДЫ OPENORCLOSE ДОЛЖНЫ БЫТЬ ПЕРЕИМЕНОВАНЫ В REFRESH (по крайнер мере там, где состояние меняется не особо понятно как (например, при нажатии Esc в геймплее))
        CloseSettingsWindowCanvas();
    }

    public void OpenSettingsWindowCanvas()
    {
        _settingsWindowCanvas.enabled = true;
        _startWindowCanvas.enabled = false;
    }

    public void CloseSettingsWindowCanvas()
    {
        _startWindowCanvas.enabled = true;
        _settingsWindowCanvas.enabled = false;
    }

    private void RefreshSlider(float value)
    {
        _musicVolumeSlider.value = value;
    }
}
