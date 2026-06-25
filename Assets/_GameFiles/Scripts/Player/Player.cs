using UnityEngine;
using UnityEngine.Events;

public sealed class Player : IDamageable, IAllRangesAttacker //я думаю, что если подобных интерфейсов для контракта какого-либо поведения страновится больше 3х, то уже нужно сделать контракт, включающий все нужные интерфейсы в себя. НО нужно понимать, обязаны ли мы при этом начинять новый единый большой интерфейс новыми методами или свойствами ИЛИ не обязаны (я думаю, что не обязаны) И также нужно понимать, какие ограничения на нас накладываются (к примеру, сможем ли мы работать с объектом на уровне его внутреннего интерфейса - ВОЗМОЖНО ЭТО КАК РАЗ КЛЮЧЕВАЯ ДЕТАТЬ, которая дает нам понять, использовать нам единый интерфейс или несколько)
{
    public static UnityAction Idled;

    private readonly PlayerMechanicStateMachine _mechanicStateMachine;

    private readonly PlayerHealthController _healthController;

    private readonly PlayerMovementController _movementController;

    private readonly PlayerOffenseController _offenseController;

    private readonly PlayerDefenseController _defenseController;

    public Player(PlayerMechanicStateMachine mechanicStateMachine, PlayerHealthController healthController, PlayerMovementController movementController, PlayerOffenseController offenseController, PlayerDefenseController defenseController)
    {
        _mechanicStateMachine = mechanicStateMachine;
        _healthController = healthController;
        _movementController = movementController;
        _offenseController = offenseController;
        _defenseController = defenseController;
    }

    //public PlayerMechanicStateMachine MechanicStateMachine => _mechanicStateMachine; //оставил так, ибо по большому счету твой класс не должен бояться такой штуки, ХОТЯ он может и не боится, но лично я не хочу же чтобы у меня эта логика была APIшкой моей модели
    public PlayerMechanicState State => _mechanicStateMachine.State; //мб стоит со всеми нижними системами сделать так же, хотя там был очень удобный доступ к событиям и деталям, НО их же можно инкапсулировать здесь

    public PlayerHealthController HealthController => _healthController;

    public PlayerMovementController MovementController => _movementController;

    public PlayerOffenseController OffenseController => _offenseController;

    public PlayerDefenseController DefenseController => _defenseController;

    public void MechanicStateUpdate()
    {
        _mechanicStateMachine.State.DoWithinFrame(this, _mechanicStateMachine);
    }

    public void Idle()
    {
        PlayerMechanicState idleState = new PlayerMechanicIdleState();

        if (_mechanicStateMachine.TrySwitchState(this, idleState) == true)
        {
            idleState.Do(this, _mechanicStateMachine);
        }
    }

    public void TakeDamage(float damage)
    {
        PlayerMechanicState stunState = new PlayerMechanicStunState(damage);

        if (_mechanicStateMachine.TrySwitchState(this, stunState) == true)
        {
            stunState.Do(this, _mechanicStateMachine);
        }
    }

    public void Die()
    {
        PlayerMechanicState deathState = new PlayerMechanicDeathState();

        if (_mechanicStateMachine.TrySwitchState(this, deathState) == true)
        {
            deathState.Do(this, _mechanicStateMachine);
        }
    }

    public void Locomote(Transform thirdPersonCameraControllerPivot, Vector2 inputDirection, Vector2 skeletonAndRenderDirection) //пропал публичный метод для бега, а я хотел дописывать контракты на ходьбу, на бег (НО МБ С FSM ВСЕ НАЛАДИТСЯ)
    {
        PlayerMechanicState locomotionState = new PlayerMechanicLocomotionState(thirdPersonCameraControllerPivot, inputDirection, skeletonAndRenderDirection);

        if (_mechanicStateMachine.TrySwitchState(this, locomotionState) == true)
        {
            locomotionState.Do(this, _mechanicStateMachine);
        }
    }

    public void Run(Transform thirdPersonCameraControllerPivot, Vector2 inputDirection, Vector2 skeletonAndRenderDirection) //пропал публичный метод для бега, а я хотел дописывать контракты на ходьбу, на бег (НО МБ С FSM ВСЕ НАЛАДИТСЯ)
    {
        PlayerMechanicState runState = new PlayerMechanicRunState(thirdPersonCameraControllerPivot, inputDirection, skeletonAndRenderDirection);

        if (_mechanicStateMachine.TrySwitchState(this, runState) == true)
        {
            runState.Do(this, _mechanicStateMachine);
        }
        //подумать про расширение - например, мне нужно будет добавить передвижение пешком, смогу ли я добавить это, соблюдая OCP?
    }

    public void Rotate(Transform thirdPersonCameraControllerPivot, Vector2 inputDirection) //(Transform renderAndSkeletonPivot, Transform thirdPersonCameraControllerPivot, Vector2 inputDirection)
    {
        _movementController.Rotate(thirdPersonCameraControllerPivot, inputDirection); //этот метод вызывается всегда, ибо игрок пока что вращается во всемя всех механик, но возможно стоит его вызывать только в механике передвижения, а в остальных механиках - не вызывать, ибо там может быть какая-то другая логика вращения (к примеру, при атаке может быть так, что игрок должен повернуться в сторону цели, а не камеры)
    }

    public void AttackCloseRange(Vector3 gameObjectPosition, Vector2 renderAndSkeletonDirectionXZ) //подумать над названием ЛК тут, ибо нам нужна семантика реальной позиции (то есть, gameObjectPosition) ИЛИ нам нужна семантика позиции для атаки (startPosition). (пример я привел неудачный, ибо тут все равно gameObjectPosition, лучше посмотреть на inputDirection сверху, где я долго писал приписку locomotion) Я ДУМАЮ ВТОРОЕ, ибо все-таки привязка к названию метода ДОЛЖНА БЫТЬ;
    {
        //возможно это стоит как-то прокидывать сверху, но пока оставлю так, ибо это логично. Но впринципе можно и прокинуть
        PlayerMechanicState attackCloseRangeState = new PlayerMechanicAttackCloseRangeState(gameObjectPosition, renderAndSkeletonDirectionXZ);

        if (_mechanicStateMachine.TrySwitchState(this, attackCloseRangeState) == true)
        {
            attackCloseRangeState.Do(this, _mechanicStateMachine);
        }
    }
    //transform.position, new Vector2(_renderAndSkeletonPivot.forward.x, _renderAndSkeletonPivot.forward.z)
    public void AttackLongRange(Vector3 gameObjectPosition, Vector2 renderAndSkeletonDirectionXZ)
    {
        PlayerMechanicState attackLongRangeState = new PlayerMechanicAttackLongRangeState(gameObjectPosition, renderAndSkeletonDirectionXZ);

        if (_mechanicStateMachine.TrySwitchState(this, attackLongRangeState) == true)
        {
            attackLongRangeState.Do(this, _mechanicStateMachine);
        }
    }

    public void Block()
    {
        PlayerMechanicState blockState = new PlayerMechanicBlockState();

        if (_mechanicStateMachine.TrySwitchState(this, blockState) == true)
        {
            blockState.Do(this, _mechanicStateMachine);
        }
    }
}
//СЮДА ИДИ