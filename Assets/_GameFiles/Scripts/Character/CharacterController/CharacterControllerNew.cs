using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public sealed class CharacterControllerNew
{
    private readonly CharacterUI _ui = new CharacterUI();

    private readonly CharacterAnimator _animator = new CharacterAnimator();

    private readonly CharacterHealthController _healthController = new CharacterHealthController();

    private readonly CharacterMovementController _movementController = new CharacterMovementController();

    private readonly CharacterOffenseController _offenseController = new CharacterOffenseController();

    private readonly CharacterDefenseController _defenseController = new CharacterDefenseController();

    ~CharacterControllerNew()
    {
        _healthController.DamageTaken -= delegate () { DamageTaken.Invoke(); };
        _healthController.DamageTaken -= RefreshUI;
        _healthController.Died -= delegate () { Died.Invoke(); };
    }

    public UnityAction DamageTaken;
    public UnityAction Died;

    public void Initialize(Image uiBar, Animator animator, float health, float locomotionSpeed, float runningSpeed, Vector3 position, Vector2 direction, WeaponCloseRange weaponCloseRange, WeaponLongRange weaponLongRange)
    {
        _ui.Initialize(uiBar);
        _animator.Initialize(animator);
        _healthController.Initialize(health);
        _movementController.Initialize(locomotionSpeed);
        _offenseController.Initialize(position, direction, weaponCloseRange, weaponLongRange);
        //_defenseController.Initialize();

        _healthController.DamageTaken += delegate () { DamageTaken.Invoke(); };
        _healthController.DamageTaken += RefreshUI;
        _healthController.Died += delegate () { Died.Invoke(); };
    }

    public void RefreshUI()
    {
        _ui.Refresh(_healthController.GetHealth()); //не Observer, но тоже неплохо
    }

    public void PlayIdleAnimation() //ВРЕМЕННАЯ МЕРА (пока нет FSM)
    {
        _animator.PlayIdle();
    }

    public void TakeDamage(float damage)
    {
        _healthController.TakeDamage(damage);
        _animator.PlayStun(); // ТАКИЕ СЕРВИСЫ БУДЕМ ПОДКЛЮЧАТЬ ЧЕРЕЗ СОБЫТИЯ (ПЕРЕПИСАТЬ ПО АНАЛОГИИ С UI)
        //_animator.PlayIdle(); //хз, почему не робит (по идее должно было быть элегантнейшим решением)
    }

    public void Die()
    {
        _healthController.Die();
    }

    public void LocomoteWithinFrame(Transform point, Vector2 locomotionDirection)
    {
        _movementController.LocomoteWithinFrame(point, locomotionDirection);
        _animator.PlayLocomotion(); //уже прям бесстыжая архитектура, надо прийти к какому то Event Bus мб, я хз. Но это очевидно событийно-ориентированная штука
    }
    /*
    public void Run(Transform point, Transform renderAndSkeletonPoint, Vector2 locomotionDirection)
    {
        _movementController.Run(point, renderAndSkeletonPoint, locomotionDirection);

        if (locomotionDirection == Vector2.zero)
        {
            _animator.PlayIdle();

            return;
        }
        _animator.PlayRunning(); //уже прям бесстыжая архитектура, надо прийти к какому то Event Bus мб, я хз. Но это очевидно событийно-ориентированная штука
    }*/

    public void AttackCloseRange(Vector3 position, Vector2 direction)
    {
        _offenseController.AttackCloseRange(position, direction);
        _animator.PlayAttackCloseRange();
    }

    public void AttackLongRange(Vector3 position, Vector2 direction)
    {
        _offenseController.AttackLongRange(position, direction);
        _animator.PlayAttackLongRange();
    }
}
