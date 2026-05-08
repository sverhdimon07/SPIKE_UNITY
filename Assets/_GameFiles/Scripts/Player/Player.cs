using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public sealed class Player : IAttacker, IDamageable //я думаю, что если подобных интерфейсов для контракта какого-либо поведения страновится больше 3х, то уже нужно сделать контракт, включающий все нужные интерфейсы в себя. НО нужно понимать, обязаны ли мы при этом начинять новый единый большой интерфейс новыми методами или свойствами ИЛИ не обязаны (я думаю, что не обязаны) И также нужно понимать, какие ограничения на нас накладываются (к примеру, сможем ли мы работать с объектом на уровне его внутреннего интерфейса - ВОЗМОЖНО ЭТО КАК РАЗ КЛЮЧЕВАЯ ДЕТАТЬ, которая дает нам понять, использовать нам единый интерфейс или несколько)
{
    private PlayerMechanicStateMachine _mechanicStateMachine;

    private PlayerHealthController _healthController;

    private PlayerMovementController _movementController;

    private PlayerOffenceController _offenseController;

    private PlayerDefenseController _defenseController;

    private bool _isRunning; //управляющие фраги - ВРЕМЕННАЯ МЕРА(пока нет FSM);

    public Player(PlayerMechanicStateMachine mechanicStateMachine, PlayerUI ui, PlayerAnimator animator, PlayerHealthController healthController, PlayerMovementController movementController, PlayerOffenceController offenseController, PlayerDefenseController defenseController, Image healthBar, Image weaponLongRangeCooldownBar, TMP_Text deathMessageText, float health, float locomotionSpeed, float runningSpeed, Vector3 position, Vector2 direction, WeaponCloseRange weaponCloseRange, WeaponLongRange weaponLongRange)
    {
        //_controller.Initialize(_healthBar, _weaponLongRangeCooldownBar, _dealthMessageText, GetComponent<Animator>(), health, locomotionSpeed, runningSpeed, transform.position, new Vector2(_renderAndSkeletonPivot.forward.x, _renderAndSkeletonPivot.forward.z), weaponCloseRange, weaponLongRange);

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

    ~Player()
    {
        _healthController.DamageTaken -= delegate () { DamageTaken.Invoke(); };
        _healthController.DamageTaken -= delegate () { _ui.RefreshHealthBar(_healthController.GetHealth()); };
        _healthController.Died -= delegate () { Died.Invoke(); };
    }

    public UnityAction DamageTaken; //под расширение (мб замедление времени во время стана делать, и возможно это делается при помощи заморозки сцены)
    public UnityAction Died;

    public float Health { get; set; }
    public float MaxHealth { get; set; }

    private void RefreshDeathMessageText() //под коммент: _healthController.Died += delegate () { _ui.RefreshDeathMessageText(); }; //надо дописать где-то вызов на выключение на старте, и включить объект в сцене
    {
        //
    }

    public void RefreshWeaponLongRangeCooldownBar()
    {
        _ui.RefreshWeaponLongRangeCooldownBar();
    }

    public void Idle() //это нужно, чтобы вернуться в Idle состояния из стана; НОРМАЛЬНАЯ, НО ВРЕМЕННАЯ МЕРА (пока нет FSM); //Изначально был метод PlayIdleAnimation, который вызывался на концах стана, НО ЭТО ВСЕ - ВРЕМЕННАЯ МЕРА (пока нет FSM)
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

    public void RotateWithinFrame(Vector2 inputDirection)
    {
        _movementController.RotateWithinFrame(_renderAndSkeletonPivot, _thirdPersonCameraControllerPivot, inputDirection);
    }

    public void LocomoteWithinFrame(Vector2 inputLocomotionDirection) //пропал публичный метод для бега, а я хотел дописывать контракты на ходьбу, на бег (НО МБ С FSM ВСЕ НАЛАДИТСЯ)
    {
        if (_mechanicStateMachine.State.GetType() != typeof(PlayerLocomotion))
        {
            return;
        }
        _mechanicStateMachine.DoDomainLogic();

        _movementController.LocomoteWithinFrame(transform, _thirdPersonCameraControllerPivot, inputLocomotionDirection, _isRunning);

        if (_isRunning == false) //из-за отсутствия MVC и событий - тут остается проверка на раннинг, хотя если бы анимации вызывались из PlayerLocomotion, проблемы бы не было
        {
            _animator.PlayLocomotionAnimation(); //это очевидно событийно-ориентированная штука, хотя и так мне тоже нравится. НО скорее всего нужно сделать по MVC как UI
        }
        else if (_isRunning == true)
        {
            _animator.PlayRunningAnimation();
        }
    }

    public void SwitchLocomotionType() //хотя вот он, своеобразный контракт для бега (ответ на коммент выше)
    {
        if (_isRunning == false)
        {
            _isRunning = true;
        }
        else if (_isRunning == true)
        {
            _isRunning = false;
        }
    }

    public void AttackCloseRange()
    {
        _offenseController.AttackCloseRange(transform.position, new Vector2(_renderAndSkeletonPivot.forward.x, _renderAndSkeletonPivot.forward.z));
        _animator.PlayAttackCloseRangeAnimation();
    }

    public void AttackLongRange()
    {
        _offenseController.AttackLongRange(transform.position, new Vector2(_renderAndSkeletonPivot.forward.x, _renderAndSkeletonPivot.forward.z)); //поработать над именованием локальных переменных внутри
        _animator.PlayAttackLongRangeAnimation();
        _ui.RefreshWeaponLongRangeCooldownBar(); //переписать на Observer, как это работает выше
    }
}
