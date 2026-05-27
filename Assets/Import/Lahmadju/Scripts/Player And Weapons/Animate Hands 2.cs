using UnityEngine;

public class AnimateHands2 : MonoBehaviour
{
    private const string GRIP = "Grip";

    [SerializeField] private Animator animator;
    public void ChangeAinmationOnGrip()
    {
        animator.SetFloat(GRIP, 1);
    }
    public void ChangeAinmationOnStatic()
    {
        animator.SetFloat(GRIP, 0);
    }
}