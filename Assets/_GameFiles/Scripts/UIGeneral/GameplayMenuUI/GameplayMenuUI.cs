using UnityEngine;
using UnityEngine.UI;

public sealed class GameplayMenuUI : MonoBehaviour
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _loadSavingButton;

    public Button ContinueButton => _continueButton;
    public Button ExitButton => _exitButton;
    public Button SaveButton => _saveButton;
    public Button LoadSavingButton => _loadSavingButton;

    public void Initialize()
    {
        OpenOrClose(); //НЕПРАВИЛЬНО, хотя когда мы полностью отрубаем канвас в геймплее, мб так и нужно (ибо вместе с этим отрубается EventSystem); Концептуально этот класс сейчас управляет своим сроком жизни, хотя такого быть не должно, НО с другой стороны это просто инит и все, НО должен ли класс вообще иметь такие приватные методы, если все то же самое я могу прописать в классе более высокого уровня
    }

    public void OpenOrClose()
    {
        if (gameObject.activeInHierarchy == false)
        {
            Open();
        }
        else if (gameObject.activeInHierarchy == true)
        {
            Close();
        }
    }

    private void Open()
    {
        gameObject.SetActive(true);
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }
}
