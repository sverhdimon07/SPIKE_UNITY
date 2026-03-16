using UnityEngine;

public class PlayerController //опять же можно избавиться от зацепок, но в контексте нашего проекта пока что это излишняя гибкость
{
    private readonly PlayerAnimator _animator = new PlayerAnimator();

    private readonly PlayerMovementController _movementController = new PlayerMovementController();

    private readonly PlayerHealthController _healthController = new PlayerHealthController();

    private readonly PlayerOffenceController _offenseController = new PlayerOffenceController();

    private readonly PlayerDefenseController _defenseController = new PlayerDefenseController();

    public void Initialize(Animator animator, float speed, WeaponCloseRange weaponCloseRange, WeaponLongRange weaponLongRange, Vector3 playerPosition, Vector2 playerDirection, Vector3 weaponPrefabScale, float weaponAttackRange)
    {
        _animator.Initialize(animator);
        _movementController.Initialize(speed);
        //_healthController.Initialize();
        _offenseController.Initialize(weaponCloseRange, weaponLongRange, playerPosition, playerDirection, weaponPrefabScale, weaponAttackRange);
        //_defenseController.Initialize();
    }

    public void Locomote(Transform playerPoint, Transform playerRenderAndSkeletonPoint, Vector2 locomotionDirection)
    {
        _movementController.Locomote(playerPoint, playerRenderAndSkeletonPoint, locomotionDirection);

        if (locomotionDirection == Vector2.zero)
        {
            _animator.PlayIdle();

            return;
        }
        _animator.PlayLocomotion(); //уже прям бесстыжая архитектура, надо прийти к какому то Event Bus мб, я хз. Но это очевидно событийно-ориентированная штука
    }

    public void Run(Transform playerPoint, Transform playerRenderAndSkeletonPoint, Vector2 locomotionDirection)
    {
        _movementController.Run(playerPoint, playerRenderAndSkeletonPoint, locomotionDirection);

        if (locomotionDirection == Vector2.zero)
        {
            _animator.PlayIdle();

            return;
        }
        _animator.PlayRunning(); //уже прям бесстыжая архитектура, надо прийти к какому то Event Bus мб, я хз. Но это очевидно событийно-ориентированная штука
    }

    public void AttackCloseRange(Vector3 playerPosition, Vector2 playerDirection)
    {
        _offenseController.AttackCloseRange(playerPosition, playerDirection);
        _animator.PlayAttackCloseRange();
    }

    public void AttackLongRange(Vector3 playerPosition, Vector2 playerDirection)
    {
        _offenseController.AttackLongRange(playerPosition, playerDirection);
        _animator.PlayAttackLongRange();
    }
}
