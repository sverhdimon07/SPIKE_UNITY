using UnityEngine;

public class CharacterAnimator //работа этого класса - идиоти€, но хз как сделать по-другому
{
    private const string IDLE = "Idle"; //знаю, что с литералами не работаем - пока не знаю другого способа (хот€ от литералов тут как раз таки уход€т через константы)
    private const string STUN = "Stun"; //к коменту выше - можно делать ссылки на сами AnimationClip'ы, провидыва€ их сюда через конструктор, а снизу где у нас дебильные сеты булов - можно плеить анимацию по ссылке на AnimationClip, но тут встает вопрос о то, насколько важно нам потер€ть услови€ переходов между анимаци€ми (эти правила перехода у нас уже учтены в нашей стейт машине, так что эти услови€ нужны ваще дл€ мега простых проектов)
    private const string DEATH = "Death";
    private const string LOCOMOTION = "Locomotion";
    private const string RUN = "Run";
    private const string ATTACK_CLOSE_RANGE = "AttackCloseRange";
    private const string ATTACK_LONG_RANGE = "AttackLongRange";

    private readonly Animator _animator;

    public CharacterAnimator(Animator animator)
    {
        _animator = animator;
    }

    public void PlayIdle()
    {
        _animator.SetBool(STUN, false);
        _animator.SetBool(DEATH, false); //пон€тно, что нелогично то, что у нас здесь есть эти строчки во всех методах, но под возможное расширение - почему бы и нет (возможно говорю бессмыслицу)
        _animator.SetBool(LOCOMOTION, false);
        _animator.SetBool(RUN, false);
        _animator.SetBool(IDLE, true);
    }

    public void PlayStun()
    {
        _animator.SetBool(IDLE, false);
        _animator.SetBool(DEATH, false);
        _animator.SetBool(LOCOMOTION, false);
        _animator.SetBool(RUN, false);
        _animator.SetBool(STUN, true);
    }

    public void PlayDeath()
    {
        _animator.SetBool(IDLE, false);
        _animator.SetBool(STUN, false);
        _animator.SetBool(LOCOMOTION, false);
        _animator.SetBool(RUN, false);
        _animator.SetBool(DEATH, true);
    }

    public void PlayLocomotion()
    {
        _animator.SetBool(IDLE, false);
        _animator.SetBool(STUN, false);
        _animator.SetBool(DEATH, false);
        _animator.SetBool(RUN, false);
        _animator.SetBool(LOCOMOTION, true);
    }

    public void PlayRun()
    {
        _animator.SetBool(IDLE, false);
        _animator.SetBool(STUN, false);
        _animator.SetBool(DEATH, false);
        _animator.SetBool(LOCOMOTION, false);
        _animator.SetBool(RUN, true);
    }

    public void PlayCloseRangeAttack()
    {
        _animator.SetTrigger(ATTACK_CLOSE_RANGE);
    }

    public void PlayLongRangeAttack()
    {
        _animator.SetTrigger(ATTACK_LONG_RANGE);
    }
    //»Ќ јѕ—”Ћя÷»ё ѕ–ќƒ”ћј“№ (здесь просто нагл€дный пример хорошей (хорошей ли?) инкапсул€ции "2го типа", когда у нас нет единого метода, в который мы можем сувать все подр€д, а у нас есть несколько методов, который создают инкапсулированный интерфейс взаимодействи€ с объектом)
}
