using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoseUI : MonoBehaviour
{
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _loadLevelButton;

    [SerializeField] private TMP_Text _statisticsText;

    public Button MenuButton => _menuButton;
    public Button LoadLevelButton => _loadLevelButton;

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

        //_statisticsText.text = "Очки - " + PlayerUI.Counter;
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }
}
