using UnityEngine;
using UnityEngine.Events;

public sealed class Player : IDamageable, IAllRangesAttacker //я думаю, что если подобных интерфейсов для контракта какого-либо поведения страновится больше 3х, то уже нужно сделать контракт, включающий все нужные интерфейсы в себя. НО нужно понимать, обязаны ли мы при этом начинять новый единый большой интерфейс новыми методами или свойствами ИЛИ не обязаны (я думаю, что не обязаны) И также нужно понимать, какие ограничения на нас накладываются (к примеру, сможем ли мы работать с объектом на уровне его внутреннего интерфейса - ВОЗМОЖНО ЭТО КАК РАЗ КЛЮЧЕВАЯ ДЕТАТЬ, которая дает нам понять, использовать нам единый интерфейс или несколько)
{
    private readonly PlayerMechanicStateMachine _mechanicStateMachine;

    private readonly PlayerHealthController _healthController;

    private readonly PlayerMovementController _movementController;

    private readonly PlayerOffenseController _offenseController;

    private readonly PlayerDefenseController _defenseController;

    private bool _isRunning; //управляющие фраги - ВРЕМЕННАЯ МЕРА(пока нет FSM);

    public Player(PlayerMechanicStateMachine mechanicStateMachine, PlayerHealthController healthController, PlayerMovementController movementController, PlayerOffenseController offenseController, PlayerDefenseController defenseController)
    {
        _mechanicStateMachine = mechanicStateMachine;
        _healthController = healthController;
        _movementController = movementController;
        _offenseController = offenseController;
        _defenseController = defenseController;

        _healthController.DamageTaken += DamageTaken;
        _healthController.Died += Died;
        _movementController.Locomoted += Locomoted;
        _movementController.Rotated += Rotated;
    }

    ~Player()
    {
        _healthController.DamageTaken -= DamageTaken;
        _healthController.Died -= Died;
        _movementController.Locomoted -= Locomoted;
        _movementController.Rotated -= Rotated;
    }

    public UnityAction StartedToIdle;
    public UnityAction DamageTaken; //под расширение (мб замедление времени во время стана делать, и возможно это делается при помощи заморозки сцены)
    public UnityAction Died;

    public UnityAction<Quaternion> Rotated;

    public UnityAction<Vector3> Locomoted;

    public Transform ThirdPersonCameraControllerPivot { get; set; }

    public Vector2 InputDirection { get; set; }

    public float Health { get; set; } //РЕАЛИЗОВАТЬ ГЕТТЕР И СЕТТЕР
    public float MaxHealth { get; set; } //РЕАЛИЗОВАТЬ ГЕТТЕР И СЕТТЕР

    public PlayerMechanicStateMachine MechanicStateMachine => _mechanicStateMachine;

    public void Idle() //это нужно, чтобы вернуться в Idle состояния из стана; НОРМАЛЬНАЯ, НО ВРЕМЕННАЯ МЕРА (пока нет FSM); //Изначально был метод PlayIdleAnimation, который вызывался на концах стана, НО ЭТО ВСЕ - ВРЕМЕННАЯ МЕРА (пока нет FSM)
    {
        //_animator.PlayIdleAnimation(); //изнутри вызвать события, поднять вызов сюда, а анимацию мы делаем уже в контроллере
    }

    public void TakeDamage(float damage)
    {
        _healthController.TakeDamage(damage);
    }

    public void Die()
    {
        _healthController.Die();
    }

    public void Locomote(Transform thirdPersonCameraControllerPivot, Vector2 inputDirection) //пропал публичный метод для бега, а я хотел дописывать контракты на ходьбу, на бег (НО МБ С FSM ВСЕ НАЛАДИТСЯ)
    {
        _mechanicStateMachine.SwitchState(this, new PlayerMechanicLocomotionState());

        if (_mechanicStateMachine.State.GetType() != typeof(PlayerLocomotion))
        {
            return;
        }
        ThirdPersonCameraControllerPivot = thirdPersonCameraControllerPivot;
        InputDirection = inputDirection;

        _mechanicStateMachine.State.DoLogic(this);

        //_movementController.Locomote(thirdPersonCameraControllerPivot, inputDirection, _isRunning);

        /*
        if (_isRunning == false) //из-за отсутствия MVC и событий - тут остается проверка на раннинг, хотя если бы анимации вызывались из PlayerLocomotion, проблемы бы не было
        {
            _animator.PlayLocomotionAnimation(); //это очевидно событийно-ориентированная штука, хотя и так мне тоже нравится. НО скорее всего нужно сделать по MVC как UI
        }
        else if (_isRunning == true)
        {
            _animator.PlayRunningAnimation();
        }*/
    }

    public void Run(Transform thirdPersonCameraControllerPivot, Vector2 inputDirection) //пропал публичный метод для бега, а я хотел дописывать контракты на ходьбу, на бег (НО МБ С FSM ВСЕ НАЛАДИТСЯ)
    {
        //подумать про расширение - например, мне нужно будет добавить передвижение пешком, смогу ли я добавить это, соблюдая OCP?
        if (_mechanicStateMachine.State.GetType() != typeof(PlayerLocomotion))
        {
            return;
        }
        ThirdPersonCameraControllerPivot = thirdPersonCameraControllerPivot;
        InputDirection = inputDirection;

        _mechanicStateMachine.State.DoLogic(this);
    }

    public void Rotate(Transform thirdPersonCameraControllerPivot, Vector2 inputDirection) //(Transform renderAndSkeletonPivot, Transform thirdPersonCameraControllerPivot, Vector2 inputDirection)
    {
        _movementController.Rotate(thirdPersonCameraControllerPivot, inputDirection);
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

    public void AttackCloseRange(Vector3 gameObjectPosition, Vector2 gameObjectDirection) //подумать над названием ЛК тут, ибо нам нужна семантика реальной позиции (то есть, gameObjectPosition) ИЛИ нам нужна семантика позиции для атаки (startPosition). (пример я привел неудачный, ибо тут все равно gameObjectPosition, лучше посмотреть на inputDirection сверху, где я долго писал приписку locomotion) Я ДУМАЮ ВТОРОЕ, ибо все-таки привязка к названию метода ДОЛЖНА БЫТЬ;
    {
        _offenseController.AttackCloseRange(gameObjectPosition, gameObjectDirection);
    }
    //transform.position, new Vector2(_renderAndSkeletonPivot.forward.x, _renderAndSkeletonPivot.forward.z)
    public void AttackLongRange(Vector3 gameObjectPosition, Vector2 gameObjectDirection)
    {
        _offenseController.AttackLongRange(gameObjectPosition, gameObjectDirection);
    }
}
