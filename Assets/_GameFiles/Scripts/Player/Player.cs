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
    
    public PlayerMechanicStateMachine MechanicStateMachine => _mechanicStateMachine;

    public PlayerHealthController HealthController => _healthController;

    public PlayerMovementController MovementController => _movementController;

    public PlayerOffenseController OffenseController => _offenseController;

    public PlayerDefenseController DefenseController => _defenseController;
    
    public void Idle()
    {
        _mechanicStateMachine.SwitchState(this, new PlayerMechanicIdleState());
        _mechanicStateMachine.State.Do(this);
    }

    public void TakeDamage(float damage)
    {
        _mechanicStateMachine.SwitchState(this, new PlayerMechanicStunState(damage));
        _mechanicStateMachine.State.Do(this);
    }

    public void Die()
    {
        _mechanicStateMachine.SwitchState(this, new PlayerMechanicDeathState());
        _mechanicStateMachine.State.Do(this);
    }

    public void Locomote(Transform thirdPersonCameraControllerPivot, Vector2 inputDirection, Vector2 skeletonAndRenderDirection) //пропал публичный метод для бега, а я хотел дописывать контракты на ходьбу, на бег (НО МБ С FSM ВСЕ НАЛАДИТСЯ)
    {
        _mechanicStateMachine.SwitchState(this, new PlayerMechanicLocomotionState(thirdPersonCameraControllerPivot, inputDirection, skeletonAndRenderDirection));
        _mechanicStateMachine.State.Do(this);
    }

    public void Run(Transform thirdPersonCameraControllerPivot, Vector2 inputDirection, Vector2 skeletonAndRenderDirection) //пропал публичный метод для бега, а я хотел дописывать контракты на ходьбу, на бег (НО МБ С FSM ВСЕ НАЛАДИТСЯ)
    {
        _mechanicStateMachine.SwitchState(this, new PlayerMechanicRunState(thirdPersonCameraControllerPivot, inputDirection, skeletonAndRenderDirection));
        _mechanicStateMachine.State.Do(this);
        //подумать про расширение - например, мне нужно будет добавить передвижение пешком, смогу ли я добавить это, соблюдая OCP?
    }

    public void Rotate(Transform thirdPersonCameraControllerPivot, Vector2 inputDirection) //(Transform renderAndSkeletonPivot, Transform thirdPersonCameraControllerPivot, Vector2 inputDirection)
    {
        _movementController.Rotate(thirdPersonCameraControllerPivot, inputDirection); //этот метод вызывается всегда, ибо игрок пока что вращается во всемя всех механик, но возможно стоит его вызывать только в механике передвижения, а в остальных механиках - не вызывать, ибо там может быть какая-то другая логика вращения (к примеру, при атаке может быть так, что игрок должен повернуться в сторону цели, а не камеры)
    }

    public void AttackCloseRange(Vector3 gameObjectPosition, Vector2 renderAndSkeletonDirectionXZ) //подумать над названием ЛК тут, ибо нам нужна семантика реальной позиции (то есть, gameObjectPosition) ИЛИ нам нужна семантика позиции для атаки (startPosition). (пример я привел неудачный, ибо тут все равно gameObjectPosition, лучше посмотреть на inputDirection сверху, где я долго писал приписку locomotion) Я ДУМАЮ ВТОРОЕ, ибо все-таки привязка к названию метода ДОЛЖНА БЫТЬ;
    {
        _mechanicStateMachine.SwitchState(this, new PlayerMechanicAttackCloseRangeState(gameObjectPosition, renderAndSkeletonDirectionXZ)); //возможно это стоит как-то прокидывать сверху, но пока оставлю так, ибо это логично. Но впринципе можно и прокинуть
        //Debug.Log("444444444444444444444444444444");
        _mechanicStateMachine.State.Do(this);
    }
    //transform.position, new Vector2(_renderAndSkeletonPivot.forward.x, _renderAndSkeletonPivot.forward.z)
    public void AttackLongRange(Vector3 gameObjectPosition, Vector2 renderAndSkeletonDirectionXZ)
    {
        _mechanicStateMachine.SwitchState(this, new PlayerMechanicAttackLongRangeState(gameObjectPosition, renderAndSkeletonDirectionXZ));
        _mechanicStateMachine.State.Do(this);
    }

    public void Block()
    {
        _mechanicStateMachine.SwitchState(this, new PlayerMechanicBlockState());
        _mechanicStateMachine.State.Do(this);
    }
}
//СЮДА ИДИ