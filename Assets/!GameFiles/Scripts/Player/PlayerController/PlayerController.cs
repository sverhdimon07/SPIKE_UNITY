using UnityEngine;
using UnityEngine.UI;

public class PlayerController //опять же можно избавиться от зацепок, но в контексте нашего проекта пока что это излишняя гибкость
{
    private readonly PlayerUI _ui = new PlayerUI();

    private readonly PlayerAnimator _animator = new PlayerAnimator();

    private readonly PlayerHealthController _healthController = new PlayerHealthController();

    private readonly PlayerMovementController _movementController = new PlayerMovementController();

    private readonly PlayerOffenceController _offenseController = new PlayerOffenceController();

    private readonly PlayerDefenseController _defenseController = new PlayerDefenseController();

    public void Initialize(Image uiBar, Animator animator, float health, float locomotionSpeed, float runningSpeed, Vector3 position, Vector2 direction, WeaponCloseRange weaponCloseRange, WeaponLongRange weaponLongRange)
    {
        _ui.Initialize(uiBar);
        _animator.Initialize(animator);
        _healthController.Initialize(health);
        _movementController.Initialize(locomotionSpeed, runningSpeed);
        _offenseController.Initialize(position, direction, weaponCloseRange, weaponLongRange);
        //_defenseController.Initialize();
    }

    public void TakeDamage(float damage)
    {
        _healthController.TakeDamage(damage);
        _ui.Refresh(_healthController.GetHealth()); //не Observer, но тоже неплохо
        _animator.PlayStun();
        //_animator.PlayIdle(); //хз, почему не робит (по идее должно было быть элегантнейшим решением)
    }

    public void PlayIdleAnimation() //ВРЕМЕННАЯ МЕРА (пока нет FSM)
    {
        _animator.PlayIdle();
    }

    public void Locomote(Transform point, Transform renderAndSkeletonPoint, Vector2 locomotionDirection)
    {
        _movementController.Locomote(point, renderAndSkeletonPoint, locomotionDirection);

        if (locomotionDirection == Vector2.zero)
        {
            _animator.PlayIdle();

            return;
        }
        _animator.PlayLocomotion(); //уже прям бесстыжая архитектура, надо прийти к какому то Event Bus мб, я хз. Но это очевидно событийно-ориентированная штука
    }

    public void Run(Transform point, Transform renderAndSkeletonPoint, Vector2 locomotionDirection)
    {
        _movementController.Run(point, renderAndSkeletonPoint, locomotionDirection);

        if (locomotionDirection == Vector2.zero)
        {
            _animator.PlayIdle();

            return;
        }
        _animator.PlayRunning(); //уже прям бесстыжая архитектура, надо прийти к какому то Event Bus мб, я хз. Но это очевидно событийно-ориентированная штука
    }

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
