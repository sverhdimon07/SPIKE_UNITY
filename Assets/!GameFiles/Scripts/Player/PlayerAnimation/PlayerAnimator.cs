using UnityEngine;

public class PlayerAnimator //работа этого класса - идиоти€, но хз как сделать по-другому
{
    private const string IDLE = "Idle"; //знаю, что с литералами не работаем - пока не знаю другого способа (хот€ от литералов тут как раз таки уход€т через константы)
    private const string STUN = "Stun";
    private const string DEATH = "Death";
    private const string LOCOMOTION = "Locomotion";
    private const string RUNNING = "Running";
    private const string ATTACK_CLOSE_RANGE = "AttackCloseRange";
    private const string ATTACK_LONG_RANGE = "AttackLongRange";

    private Animator _animator;

    public void Initialize(Animator animator)
    {
        _animator = animator;
    }

    public void PlayIdleAnimation()
    {
        _animator.SetBool(STUN, false);
        _animator.SetBool(DEATH, false); //пон€тно, что нелогично то, что у нас здесь есть эти строчки во всех методах, но под возможное расширение - почему бы и нет (возможно говорю бессмыслицу)
        _animator.SetBool(LOCOMOTION, false);
        _animator.SetBool(RUNNING, false);
        _animator.SetBool(IDLE, true);
    }

    public void PlayStunAnimation()
    {
        _animator.SetBool(IDLE, false);
        _animator.SetBool(DEATH, false);
        _animator.SetBool(LOCOMOTION, false);
        _animator.SetBool(RUNNING, false);
        _animator.SetBool(STUN, true);
    }

    public void PlayDeathAnimation()
    {
        _animator.SetBool(IDLE, false);
        _animator.SetBool(STUN, false);
        _animator.SetBool(LOCOMOTION, false);
        _animator.SetBool(RUNNING, false);
        _animator.SetBool(DEATH, true);
    }

    public void PlayLocomotionAnimation()
    {
        _animator.SetBool(IDLE, false);
        _animator.SetBool(STUN, false);
        _animator.SetBool(DEATH, false);
        _animator.SetBool(RUNNING, false);
        _animator.SetBool(LOCOMOTION, true);
    }

    public void PlayRunningAnimation()
    {
        _animator.SetBool(IDLE, false);
        _animator.SetBool(STUN, false);
        _animator.SetBool(DEATH, false);
        _animator.SetBool(LOCOMOTION, false);
        _animator.SetBool(RUNNING, true);
    }

    public void PlayAttackCloseRangeAnimation()
    {
        _animator.SetTrigger(ATTACK_CLOSE_RANGE);
    }

    public void PlayAttackLongRangeAnimation()
    {
        _animator.SetTrigger(ATTACK_LONG_RANGE);
    }
    //»Ќ јѕ—”Ћя÷»ё ѕ–ќƒ”ћј“№ (здесь просто нагл€дный пример хорошей (хорошей ли?) инкапсул€ции "2го типа", когда у нас нет единого метода, в который мы можем сувать все подр€д, а у нас есть несколько методов, который создают инкапсулированный интерфейс взаимодействи€ с объектом)
}
