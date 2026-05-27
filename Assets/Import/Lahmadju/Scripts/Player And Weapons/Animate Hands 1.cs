using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHands1 : MonoBehaviour
{
    private const string GRIP = "Grip";
    private const string TRIGGER = "Trigger";

    [SerializeField] private Animator animator;

    [SerializeField] private InputActionProperty gripAnimationAction;
    [SerializeField] private InputActionProperty triggerAnimationAction;
    private void Update()
    {
        float gripValue = gripAnimationAction.action.ReadValue<float>();
        animator.SetFloat(GRIP, gripValue);

        float triggerValue = triggerAnimationAction.action.ReadValue<float>();
        animator.SetFloat(TRIGGER, triggerValue);
    }
}