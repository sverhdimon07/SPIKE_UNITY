using System.Threading.Tasks;
using Unity.Cinemachine;
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

    private readonly Transform _fighter;
    private readonly GameObject _hud;
    private readonly CinemachinePositionComposer _cameraComposer;
    private readonly Vector3 _humanCameraTargetOffset;
    private readonly float _humanCameraDistance;
    private readonly float _fighterCameraDistance;
    private int _attackPresentationVersion;

    public PlayerView(PlayerUI ui, PlayerAnimator animator, Transform gameObjectPivot, Transform renderAndSkeletonPivot, ParticleSystem closeRangeWeaponEffect, ParticleSystem longRangeWeaponEffect, AudioSource closeRangeWeaponSound, AudioSource longRangeWeaponSound, Transform fighter, GameObject hud,
        CinemachinePositionComposer cameraComposer, float fighterCameraDistance) //тут слишком конкретные классы лежат, надо их сделать общими сервисами для всех - и игрока, и врагов
    {
        _cameraComposer = cameraComposer;
        _humanCameraTargetOffset = cameraComposer != null ? cameraComposer.TargetOffset : Vector3.zero;
        _humanCameraDistance = cameraComposer != null ? cameraComposer.CameraDistance : 0f;
        _fighterCameraDistance = fighterCameraDistance;
        _fighter = fighter;
        _hud = hud;
        _ui = ui;
        _animator = animator;
        _gameObjectPivot = gameObjectPivot;
        _renderAndSkeletonPivot = renderAndSkeletonPivot;
        _closeRangeWeaponEffect = closeRangeWeaponEffect;
        _longRangeWeaponEffect = longRangeWeaponEffect;
        _closeRangeWeaponSound = closeRangeWeaponSound;
        _longRangeWeaponSound = longRangeWeaponSound;
    }

    public void PresentLives(int value)
    {
        _ui.RefreshLivesBar(value);
    }

    public void PresentScoreIncrease()
    {
        _ui.RefreshCounterText();
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
        Transform visibleModel = _fighter != null && _fighter.gameObject.activeSelf ? _fighter : _renderAndSkeletonPivot;
        visibleModel.rotation = requiredWorldRotation;
    }

    public Quaternion FighterRotation => _fighter.rotation;

    public void MoveAirborne(Vector3 position)
    {
        _gameObjectPivot.position = position;
    }

    public void SetFighterForm(bool isFighter, Vector3 cameraForward)
    {
        if (_fighter == null || _hud == null)
        {
            return;
        }

        if (isFighter)
        {
            CancelAttackPresentation();
            Vector3 facing = Vector3.ProjectOnPlane(cameraForward, Vector3.up);
            _fighter.rotation = Quaternion.LookRotation(facing);
        }
        else
        {
            Vector3 facing = Vector3.ProjectOnPlane(_fighter.forward, Vector3.up);
            if (facing.sqrMagnitude > 0.0001f)
            {
                _renderAndSkeletonPivot.rotation = Quaternion.LookRotation(facing);
            }
        }

        if (_cameraComposer != null)
        {
            _cameraComposer.TargetOffset = isFighter
                ? Quaternion.Inverse(_gameObjectPivot.rotation) * (_fighter.position - _gameObjectPivot.position)
                : _humanCameraTargetOffset;
            _cameraComposer.CameraDistance = isFighter ? _fighterCameraDistance : _humanCameraDistance;
        }

        _fighter.gameObject.SetActive(isFighter);
        _renderAndSkeletonPivot.gameObject.SetActive(!isFighter);
        _hud.SetActive(!isFighter);
    }

    public void CancelAttackPresentation()
    {
        _attackPresentationVersion++;
        _closeRangeWeaponEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _longRangeWeaponEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _closeRangeWeaponSound.Stop();
        _longRangeWeaponSound.Stop();
        _ui.RefreshWeaponLongRangeCooldownBarOnFull();
    }

    public async Task PresentCloseRangeAttack()
    {
        int presentationVersion = _attackPresentationVersion;
        _animator.PlayCloseRangeAttack();
        await Task.Delay(100);
        if (presentationVersion != _attackPresentationVersion)
        {
            return;
        }

        _closeRangeWeaponEffect.Play();
        _closeRangeWeaponSound.Play();
    }

    public async Task PresentLongRangeAttack()
    {
        int presentationVersion = _attackPresentationVersion;
        _ui.RefreshWeaponLongRangeCooldownBarOnEmpty();
        _animator.PlayLongRangeAttack();
        await Task.Delay(900);
        if (presentationVersion != _attackPresentationVersion)
        {
            return;
        }

        _longRangeWeaponEffect.Play();
        _longRangeWeaponSound.Play();
        await Task.Delay(600);
        if (presentationVersion == _attackPresentationVersion)
        {
            _ui.RefreshWeaponLongRangeCooldownBarOnFull();
        }
    }

    public void PresentBlock()
    {
        _animator.PlayBlock();
    }
}
