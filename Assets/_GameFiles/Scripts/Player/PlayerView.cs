using System.Threading.Tasks;
using UnityEngine;

public sealed class PlayerView //контракты на обновление UI, обновление анимации - хз, нужны ли
{
    private readonly PlayerUI _ui;

    private readonly PlayerAnimator _animator;

    private readonly Transform _gameObjectPivot;

    private readonly Transform _renderAndSkeletonPivot;

    private readonly ParticleSystem _closeRangeWeaponEffect;
    private readonly ParticleSystem _longRangeWeaponEffect;

    private readonly AudioSource _closeRangeWeaponSound;
    private readonly AudioSource _longRangeWeaponSound;

    public PlayerView(PlayerUI ui, PlayerAnimator animator, Transform gameObjectPivot, Transform renderAndSkeletonPivot, ParticleSystem closeRangeWeaponEffect, ParticleSystem longRangeWeaponEffect, AudioSource closeRangeWeaponSound, AudioSource longRangeWeaponSound) //тут слишком конкретные классы лежат, надо их сделать общими сервисами для всех - и игрока, и врагов
    {
        _ui = ui;
        _animator = animator;
        _gameObjectPivot = gameObjectPivot;
        _renderAndSkeletonPivot = renderAndSkeletonPivot;
        _closeRangeWeaponEffect = closeRangeWeaponEffect;
        _longRangeWeaponEffect = longRangeWeaponEffect;
        _closeRangeWeaponSound = closeRangeWeaponSound;
        _longRangeWeaponSound = longRangeWeaponSound;
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

    public async Task PresentCloseRangeAttack()
    {
        _animator.PlayCloseRangeAttack();
        await Task.Delay(100);
        _closeRangeWeaponEffect.Play();
        _closeRangeWeaponSound.Play();
    }

    public async Task PresentLongRangeAttack()
    {
        _ui.RefreshWeaponLongRangeCooldownBarOnEmpty();
        _animator.PlayLongRangeAttack();
        await Task.Delay(900);
        _longRangeWeaponEffect.Play();
        _longRangeWeaponSound.Play();
        await _ui.RefreshWeaponLongRangeCooldownBarOnFull();
    }

    public void PresentScoreIncrease()
    {
        _ui.RefreshCounterText();
    }
}
