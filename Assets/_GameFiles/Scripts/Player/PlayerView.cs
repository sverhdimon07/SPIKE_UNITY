using UnityEngine;

public sealed class PlayerView //контракты на обновление UI, обновление анимации - хз, нужны ли
{
    private readonly PlayerUI _ui;

    private readonly PlayerAnimator _animator;

    private readonly Transform _characterModel;

    public PlayerView(PlayerUI ui, PlayerAnimator animator, Transform characterModel) //тут слишком конкретные классы лежат, надо их сделать общими сервисами для всех - и игрока, и врагов
    {
        _ui = ui;
        _animator = animator;
        _characterModel = characterModel;
    }

    public void PresentIdle()
    {
        //
        _animator.PlayIdle();
    }

    public void PresentDamageTake(float valueLevel)
    {
        _ui.RefreshHealthBar(valueLevel);
        _animator.PlayStun();
        //_animator.PlayIdle(); //хз, почему не робит (по идее должно было быть элегантнейшим решением), но раз не робит, то переключаться в состояние айдла надо после окончания анимации стана
    }

    public void PresentWeaponLongRangeCooldown()
    {
        _ui.RefreshWeaponLongRangeCooldownBar();
        //
    }

    public void PresentDeath()
    {
        _ui.RefreshDeathMessageText();
        //
    }

    public void TurnCharacterModel(Quaternion requiredWorldRotation) //ИНКАПСУЛЯЦИЯ - нужна ли, ибо у нас значения уже верные приходят. Но по правилам - да, надо. Но по логике - хз, ибо этот класс изменяется исключительно после изменения модели
    {
        _characterModel.rotation = requiredWorldRotation;
    }

    public void MoveCharacterModel(Vector3 requiredWorldPosition)
    {
        _characterModel.position = requiredWorldPosition;
    }

    public void PresentCloseRangeAttack()
    {
        _animator.PlayCloseRangeAttack();
    }

    public void PresentLongRangeAttack()
    {
        _animator.PlayLongRangeAttack();
        _ui.RefreshWeaponLongRangeCooldownBar();
    }
}
