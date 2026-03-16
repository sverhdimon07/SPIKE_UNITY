using UnityEngine;

public class PlayerAnimator
{
    private const string IDLE = "Idle"; //знаю, что с литералами не работаем - пока не знаю другого способа
    private const string LOCOMOTION = "Locomotion";
    private const string RUNNING = "Running";
    private const string ATTACK_CLOSE_RANGE = "AttackCloseRange";
    private const string ATTACK_LONG_RANGE = "AttackLongRange";

    private Animator _animator;

    public void Initialize(Animator animator)
    {
        _animator = animator;
    }

    public void PlayIdle()
    {
        _animator.SetBool(LOCOMOTION, false); //идиоти€, но хз как работать по-другому
        _animator.SetBool(RUNNING, false);
        _animator.SetBool(IDLE, true);
    }

    public void PlayLocomotion()
    {
        _animator.SetBool(IDLE, false); //идиоти€, но хз как работать по-другому
        _animator.SetBool(RUNNING, false);
        _animator.SetBool(LOCOMOTION, true);
    }

    public void PlayRunning() //идиоти€, но хз как работать по-другому
    {
        _animator.SetBool(IDLE, false);
        _animator.SetBool(LOCOMOTION, false);
        _animator.SetBool(RUNNING, true);
    }

    public void PlayAttackCloseRange()
    {
        _animator.SetTrigger(ATTACK_CLOSE_RANGE);
    }

    public void PlayAttackLongRange()
    {
        _animator.SetTrigger(ATTACK_LONG_RANGE);
    }
    //работа с инкапсул€цией!
}
