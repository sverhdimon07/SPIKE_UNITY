using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinUI : MonoBehaviour
{
    [SerializeField] private Button _menuButton;

    [SerializeField] private TMP_Text _statisticsText;

    private InputController _inputController;

    public Button MenuButton => _menuButton;

    private void Awake()
    {
        _inputController = FindAnyObjectByType<InputController>();

        Initialize();
    }

    public void Initialize()
    {
        gameObject.SetActive(false);
        //OpenOrClose(); //НЕПРАВИЛЬНО, хотя когда мы полностью отрубаем канвас в геймплее, мб так и нужно (ибо вместе с этим отрубается EventSystem); Концептуально этот класс сейчас управляет своим сроком жизни, хотя такого быть не должно, НО с другой стороны это просто инит и все, НО должен ли класс вообще иметь такие приватные методы, если все то же самое я могу прописать в классе более высокого уровня
    }

    public void OpenOrClose()
    {
        _inputController.OnOpeningGameplayMenuButtonPressed();

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
        if (ScoreController.Score == 2)
        {
            return;
        }
        gameObject.SetActive(true);
        
        _statisticsText.text = "Очки: " + ScoreController.Score;
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }
}
