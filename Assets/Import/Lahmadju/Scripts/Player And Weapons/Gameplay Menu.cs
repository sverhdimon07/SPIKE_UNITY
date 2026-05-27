using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameplayMenu : MonoBehaviour
{
    [SerializeField] private UnityEvent menuOpened;
    private bool menuOpenedState;
    [SerializeField] private UnityEvent menuClosed;
    private bool menuClosedState;

    [SerializeField] InputActionProperty leftTriggerAction;
    [SerializeField] InputActionProperty rightTriggerAction;
    [SerializeField] InputActionProperty rightGripAction;
    private void Update()
    {
        float leftTriggerValue = leftTriggerAction.action.ReadValue<float>();
        float rightTriggerValue = rightTriggerAction.action.ReadValue<float>();
        float rightGripValue = rightGripAction.action.ReadValue<float>();

        if ((leftTriggerValue > 0f) && (rightTriggerValue > 0f))
        {
            if (menuOpenedState == false)
            {
                menuOpened.Invoke();
            }
            if (rightGripValue > 0f)
            {
                PlayerHealthSystem.HealthRegeneration();
                PlayerHealthSystem.FillPlayerLives();

                BossHealthSystem.HealthRegeneration();
                BossHealthSystem.SecondHealthRegeneration();
                BossHealthSystem.FillBossLives();
                SceneManager.LoadScene("Menu Scene");
            }
        }
        else if ((leftTriggerValue == 0f) && (rightTriggerValue == 0f))
        {
            if (menuClosedState == false)
            {
                menuClosed.Invoke();
            }
        }
    }
}