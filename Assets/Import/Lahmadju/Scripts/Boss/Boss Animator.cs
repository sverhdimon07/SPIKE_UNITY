using UnityEngine;
using UnityEngine.Events;

public class BossAnimator : MonoBehaviour
{
    private const string STRAIGHT_ATTACK = "StraightAttack";
    private const string RIGHT_ATTACK = "RightAttack";
    private const string LEFT_ATTACK = "LeftAttack";
    private const string BOTTOM_ATTACK = "BottomAttack";
    private const string STAGGER_ATTACK = "Stagger";
    private const string BLOCK = "Block";
    private const string WALKING = "Walking";

    [SerializeField] private Animator animator;

    [SerializeField] private UnityEvent hitHandler1Activated;
    [SerializeField] private UnityEvent hitHandler2Activated;
    [SerializeField] private UnityEvent hitHandler3Activated;
    [SerializeField] private UnityEvent hitHandler4Activated;
    public void PlayRandomAttack()
    {
        int number = Random.Range(1,4+1);

        if (number == 1)
        {
            PlayStraightAttack();
        }
        else if (number == 2)
        {
            PlayRightAttack();
        }
        else if (number == 3)
        {
            PlayLeftAttack();
        }
        else if (number == 4)
        {
            PlayBottomAttack();
        }
    }
    public void PlayStraightAttack()
    {
        animator.SetTrigger(STRAIGHT_ATTACK);
        hitHandler1Activated.Invoke();
    }
    public void PlayRightAttack()
    {
        animator.SetTrigger(RIGHT_ATTACK);
        hitHandler4Activated.Invoke();
    }
    public void PlayLeftAttack()
    {
        animator.SetTrigger(LEFT_ATTACK);
        hitHandler3Activated.Invoke();
    }
    public void PlayBottomAttack()
    {
        animator.SetTrigger(BOTTOM_ATTACK);
        hitHandler2Activated.Invoke();
    }
    public void PlayStagger()
    {
        animator.SetTrigger(STAGGER_ATTACK);
    }
    public void ControlWalkingAnimation(bool condition)
    {
        animator.SetBool(WALKING, condition);
    }
    public void ControlBlockAnimation(bool condition)
    {
        animator.SetBool(BLOCK, condition);
    }
}