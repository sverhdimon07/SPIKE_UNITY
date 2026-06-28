using UnityEngine;
using UnityEngine.Events;

public /*abstract*/ class Character : IDamageable, IAllRangesAttacker //я думаю, что если подобных интерфейсов для контракта какого-либо поведения страновится больше 3х, то уже нужно сделать контракт, включающий все нужные интерфейсы в себя. НО нужно понимать, обязаны ли мы при этом начинять новый единый большой интерфейс новыми методами или свойствами ИЛИ не обязаны (я думаю, что не обязаны) И также нужно понимать, какие ограничения на нас накладываются (к примеру, сможем ли мы работать с объектом на уровне его внутреннего интерфейса - ВОЗМОЖНО ЭТО КАК РАЗ КЛЮЧЕВАЯ ДЕТАТЬ, которая дает нам понять, использовать нам единый интерфейс или несколько)
{
    public UnityAction Idled;

    private readonly CharacterMechanicStateMachine _mechanicStateMachine;

    private readonly CharacterHealthController _healthController;

    private readonly CharacterMovementController _movementController;

    private readonly CharacterOffenseController _offenseController;

    private readonly CharacterDefenseController _defenseController;

    public Character(CharacterMechanicStateMachine mechanicStateMachine, CharacterHealthController healthController, CharacterMovementController movementController, CharacterOffenseController offenseController, CharacterDefenseController defenseController)
    {
        _mechanicStateMachine = mechanicStateMachine;
        _healthController = healthController;
        _movementController = movementController;
        _offenseController = offenseController;
        _defenseController = defenseController;
    }

    //public CharacterMechanicStateMachine MechanicStateMachine => _mechanicStateMachine; //оставил так, ибо по большому счету твой класс не должен бояться такой штуки, ХОТЯ он может и не боится, но лично я не хочу же чтобы у меня эта логика была APIшкой моей модели
    public CharacterMechanicState State => _mechanicStateMachine.State; //мб стоит со всеми нижними системами сделать так же, хотя там был очень удобный доступ к событиям и деталям, НО их же можно инкапсулировать здесь

    public CharacterHealthController HealthController => _healthController;

    public CharacterMovementController MovementController => _movementController;

    public CharacterOffenseController OffenseController => _offenseController;

    public CharacterDefenseController DefenseController => _defenseController;

    public void MechanicStateUpdate()
    {
        _mechanicStateMachine.State.DoWithinFrame(this, _mechanicStateMachine);
    }

    public void Idle()
    {
        CharacterMechanicState idleState = new CharacterMechanicIdleState();

        if (_mechanicStateMachine.TrySwitchState(this, idleState) == true)
        {
            idleState.Do(this, _mechanicStateMachine);
        }
    }

    public void TakeDamage(float damage)
    {
        CharacterMechanicState stunState = new CharacterMechanicStunState(damage);

        if (_mechanicStateMachine.TrySwitchState(this, stunState) == true)
        {
            stunState.Do(this, _mechanicStateMachine);
        }
    }

    public void Die()
    {
        CharacterMechanicState deathState = new CharacterMechanicDeathState();

        if (_mechanicStateMachine.TrySwitchState(this, deathState) == true)
        {
            deathState.Do(this, _mechanicStateMachine);
        }
    }

    public void Locomote(/*Transform thirdPersonCameraControllerPivot, */Vector2 inputDirection) //пропал публичный метод для бега, а я хотел дописывать контракты на ходьбу, на бег (НО МБ С FSM ВСЕ НАЛАДИТСЯ)
    {
        CharacterMechanicState locomotionState = new CharacterMechanicLocomotionState(/*thirdPersonCameraControllerPivot, */inputDirection);

        if (_mechanicStateMachine.TrySwitchState(this, locomotionState) == true)
        {
            locomotionState.Do(this, _mechanicStateMachine);
        }
    }

    public void Run(/*Transform thirdPersonCameraControllerPivot, */Vector2 inputDirection) //пропал публичный метод для бега, а я хотел дописывать контракты на ходьбу, на бег (НО МБ С FSM ВСЕ НАЛАДИТСЯ)
    {
        CharacterMechanicState runState = new CharacterMechanicRunState(/*thirdPersonCameraControllerPivot, */inputDirection);

        if (_mechanicStateMachine.TrySwitchState(this, runState) == true)
        {
            runState.Do(this, _mechanicStateMachine);
        }
        //подумать про расширение - например, мне нужно будет добавить передвижение пешком, смогу ли я добавить это, соблюдая OCP?
    }

    public void Rotate(/*Transform thirdPersonCameraControllerPivot, */Vector2 inputDirection) //(Transform renderAndSkeletonPivot, Transform thirdPersonCameraControllerPivot, Vector2 inputDirection)
    {
        _movementController.Rotate(/*thirdPersonCameraControllerPivot, */inputDirection); //этот метод вызывается всегда, ибо игрок пока что вращается во всемя всех механик, но возможно стоит его вызывать только в механике передвижения, а в остальных механиках - не вызывать, ибо там может быть какая-то другая логика вращения (к примеру, при атаке может быть так, что игрок должен повернуться в сторону цели, а не камеры)
    }

    public void AttackCloseRange(Vector3 gameObjectPosition, Vector2 renderAndSkeletonDirectionXZ) //подумать над названием ЛК тут, ибо нам нужна семантика реальной позиции (то есть, gameObjectPosition) ИЛИ нам нужна семантика позиции для атаки (startPosition). (пример я привел неудачный, ибо тут все равно gameObjectPosition, лучше посмотреть на inputDirection сверху, где я долго писал приписку locomotion) Я ДУМАЮ ВТОРОЕ, ибо все-таки привязка к названию метода ДОЛЖНА БЫТЬ;
    {
        //возможно это стоит как-то прокидывать сверху, но пока оставлю так, ибо это логично. Но впринципе можно и прокинуть
        CharacterMechanicState attackCloseRangeState = new CharacterMechanicAttackCloseRangeState(gameObjectPosition, renderAndSkeletonDirectionXZ);

        if (_mechanicStateMachine.TrySwitchState(this, attackCloseRangeState) == true)
        {
            attackCloseRangeState.Do(this, _mechanicStateMachine);
        }
    }
    //transform.position, new Vector2(_renderAndSkeletonPivot.forward.x, _renderAndSkeletonPivot.forward.z)
    public void AttackLongRange(Vector3 gameObjectPosition, Vector2 renderAndSkeletonDirectionXZ)
    {
        CharacterMechanicState attackLongRangeState = new CharacterMechanicAttackLongRangeState(gameObjectPosition, renderAndSkeletonDirectionXZ);

        if (_mechanicStateMachine.TrySwitchState(this, attackLongRangeState) == true)
        {
            attackLongRangeState.Do(this, _mechanicStateMachine);
        }
    }
}
