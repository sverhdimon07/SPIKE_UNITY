using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public sealed class PlayerController //опять же можно избавиться от зацепок, но в контексте нашего проекта пока что это излишняя гибкость
{
    private readonly PlayerUI _ui = new PlayerUI(); //ПОДКЛЮЧИТЬ UI ТУТ, А НЕ В EVENT BUS

    private readonly PlayerAnimator _animator = new PlayerAnimator();

    private readonly PlayerHealthController _healthController = new PlayerHealthController();

    private readonly PlayerMovementController _movementController = new PlayerMovementController();

    private readonly PlayerOffenceController _offenseController = new PlayerOffenceController();

    private readonly PlayerDefenseController _defenseController = new PlayerDefenseController();

    ~PlayerController()
    {
        _healthController.DamageTaken -= delegate () { DamageTaken.Invoke(); };
        _healthController.DamageTaken -= delegate () { _ui.RefreshHealthBar(_healthController.GetHealth()); };
        _healthController.Died -= delegate () { Died.Invoke(); };
    }

    public UnityAction DamageTaken;
    public UnityAction Died;

    public void Initialize(Image healthBar, Image weaponLongRangeCooldownBar, TMP_Text deathMessageText, Animator animator, float health, float locomotionSpeed, float runningSpeed, Vector3 position, Vector2 direction, WeaponCloseRange weaponCloseRange, WeaponLongRange weaponLongRange)
    {
        _ui.Initialize(healthBar, weaponLongRangeCooldownBar, deathMessageText);
        _animator.Initialize(animator);
        _healthController.Initialize(health);
        _movementController.Initialize(locomotionSpeed, runningSpeed);
        _offenseController.Initialize(position, direction, weaponCloseRange, weaponLongRange);
        //_defenseController.Initialize();

        _healthController.DamageTaken += delegate () { DamageTaken.Invoke(); };
        _healthController.DamageTaken += delegate () { _ui.RefreshHealthBar(_healthController.GetHealth()); };
        _healthController.Died += delegate () { Died.Invoke(); };
        _healthController.Died += delegate () { _ui.RefreshDeathMessageText(); }; //надо дописать где-то вызов на выключение на старте, и включить объект в сцене
    }

    public void RefreshDeathMessageText() //под коммент выше
    {
        //
    }

    public void RefreshWeaponLongRangeCooldownBar()
    {
        _ui.RefreshWeaponLongRangeCooldownBar();
    }

    public void Idle() //Изначально был метод PlayIdleAnimation, который вызывался на концах стана, НО ЭТО ВСЕ - ВРЕМЕННАЯ МЕРА (пока нет FSM)
    {
        _animator.PlayIdleAnimation();
    }

    public void TakeDamage(float damage)
    {
        _healthController.TakeDamage(damage);
        _animator.PlayStunAnimation(); // ТАКИЕ СЕРВИСЫ БУДЕМ ПОДКЛЮЧАТЬ ЧЕРЕЗ СОБЫТИЯ (ПЕРЕПИСАТЬ ПО АНАЛОГИИ С UI)
        //_animator.PlayIdle(); //хз, почему не робит (по идее должно было быть элегантнейшим решением)
    }

    public void Die()
    {
        _healthController.Die();
    }

    public void RotateWithinFrame(Transform renderAndSkeletonPoint, Transform cameraPoint, Vector2 inputDirection)
    {
        _movementController.RotateWithinFrame(renderAndSkeletonPoint, cameraPoint, inputDirection);
    }

    public void LocomoteWithinFrame(Transform point, Transform cameraPoint, Vector2 inputLocomotionDirection, bool isRunning)
    {
        _movementController.LocomoteWithinFrame(point, cameraPoint, inputLocomotionDirection, isRunning);

        if (isRunning == false) //из-за отсутствия MVC и событий - тут остается проверка на раннинг, хотя если бы анимации вызывались из PlayerLocomotion, проблемы бы не было
        {
            _animator.PlayLocomotionAnimation(); //это очевидно событийно-ориентированная штука, хотя и так мне тоже нравится. НО скорее всего нужно сделать по MVC как UI
        }
        else if (isRunning == true)
        {
            _animator.PlayRunningAnimation();
        }
    }

    public void AttackCloseRange(Vector3 position, Vector2 direction)
    {
        _offenseController.AttackCloseRange(position, direction);
        _animator.PlayAttackCloseRangeAnimation();
    }

    public void AttackLongRange(Vector3 position, Vector2 direction)
    {
        _offenseController.AttackLongRange(position, direction);
        _animator.PlayAttackLongRangeAnimation();
        _ui.RefreshWeaponLongRangeCooldownBar(); //переписать на Observer, как это работает выше
    }
}
