using UnityEngine; //нужен ли MonoBehaviour?

public class PlayerController //опять же можно избавиться от зацепок, но в контексте нашего проекта пока что это излишняя гибкость
{
    private readonly PlayerAnimator _animator = new PlayerAnimator();

    private readonly PlayerMovementController _movementController = new PlayerMovementController();

    private readonly PlayerHealthController _healthController = new PlayerHealthController();

    private readonly PlayerOffenceController _offenseController = new PlayerOffenceController();

    private readonly PlayerDefenseController _defenseController = new PlayerDefenseController();

    private readonly PlayerWeaponController _weaponController = new PlayerWeaponController();

    public void Initialize(Animator animator, float speed, Weapon weapon)
    {
        InitializeAnimator(animator);
        InitializeMovementController(speed);
        InitializeHealthController();
        InitializeOffenceController(weapon);
        InitializeDefenseController();
        InitializeWeaponController();
    }

    public void Locomote(Transform point, Vector2 direction)
    {
        _movementController.Locomote(point, direction);

        if (direction == Vector2.zero)
        {
            _animator.PlayIdle();

            return;
        }
        _animator.PlayLocomotion(); //уже прям бесстыжая архитектура, надо прийти к какому то Event Bus мб, я хз. Но это очевидно событийно-ориентированная штука
    }

    public void Run(Transform point, Vector2 direction)
    {
        _movementController.Run(point, direction);

        if (direction == Vector2.zero)
        {
            _animator.PlayIdle();

            return;
        }
        _animator.PlayRunning(); //уже прям бесстыжая архитектура, надо прийти к какому то Event Bus мб, я хз. Но это очевидно событийно-ориентированная штука
    }

    public void Attack(IAttack attack, IDamageCalculator damageCalculator, Weapon weapon, Character damageReciever)
    {
        _offenseController.Attack(attack, damageCalculator, weapon, damageReciever);

        if (attack.GetType() == typeof(PlayerAttackCloseRange)) //уже прям бесстыжая архитектура, надо прийти к какому то Event Bus мб, я хз
        {
            _animator.PlayAttackCloseRange();
        }
        else if (attack.GetType() == typeof(PlayerAttackLongRange))
        {
            _animator.PlayAttackLongRange();
        }
    }

    private void InitializeAnimator(Animator animator)
    {
        _animator.Initialize(animator);
    }

    private void InitializeMovementController(float speed)
    {
        _movementController.Initialize(speed);
    }

    private void InitializeHealthController()
    {
        //_healthController.Initialize();
    }

    private void InitializeOffenceController(Weapon weapon)
    {
        _offenseController.Initialize(weapon);
    }

    private void InitializeDefenseController()
    {
        //
    }

    private void InitializeWeaponController()
    {
        //_weaponController.Initialize();
    }
}
