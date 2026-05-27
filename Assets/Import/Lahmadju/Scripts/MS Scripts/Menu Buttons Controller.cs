using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuButtonsController : MonoBehaviour
{
    [SerializeField] InputActionProperty leftGripAction;
    [SerializeField] InputActionProperty rightGripAction;
    private void Update()
    {
        float leftGripValue = leftGripAction.action.ReadValue<float>();
        float rightGripValue = rightGripAction.action.ReadValue<float>();

        if (leftGripValue > 0f) 
        {
            SceneManager.LoadScene("Bossfight Scene");
        }

        if (rightGripValue > 0f)
        {
            Application.Quit();
        }
    }
}