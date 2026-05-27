using UnityEngine;

public sealed class PlayerView //контракты на обновление UI, обновление анимации - хз, нужны ли
{
    private readonly PlayerUI _ui;

    private readonly PlayerAnimator _animator;

    private readonly Transform _gameObjectPivot;

    private readonly Transform _renderAndSkeletonPivot;

    public PlayerView(PlayerUI ui, PlayerAnimator animator, Transform gameObjectPivot, Transform renderAndSkeletonPivot) //тут слишком конкретные классы лежат, надо их сделать общими сервисами для всех - и игрока, и врагов
    {
        _ui = ui;
        _animator = animator;
        _gameObjectPivot = gameObjectPivot;
        _renderAndSkeletonPivot = renderAndSkeletonPivot;
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

    public void MoveCharacterModelInLocomotionForm(Vector3 requiredWorldPosition)
    {
        _gameObjectPivot.position = requiredWorldPosition;
        _animator.PlayLocomotion();
    }

    public void MoveCharacterModelInRunForm(Vector3 requiredWorldPosition)
    {
        _gameObjectPivot.position = requiredWorldPosition;
        _animator.PlayRun();
    }

    public void TurnCharacterModel(Quaternion requiredWorldRotation) //ИНКАПСУЛЯЦИЯ - нужна ли, ибо у нас значения уже верные приходят. Но по правилам - да, надо. Но по логике - хз, ибо этот класс изменяется исключительно после изменения модели
    {
        _renderAndSkeletonPivot.rotation = requiredWorldRotation;
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
