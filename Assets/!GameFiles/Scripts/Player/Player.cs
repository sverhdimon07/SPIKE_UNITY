using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour, IAttackerCloseRange, IAttackerLongRange, IDamageable //я бы еще накидал контрактов на управляемое перемещение
{
    [SerializeField] private Transform _renderAndSkeletonPoint;

    private readonly PlayerController _controller = new PlayerController(); //можно использовать DI, но пока что это излишняя гибкость

    private void Update()
    {
        //возможно здесь будем корректировать то, куда смотрит ГГ (но возможно это стоит делать не здесь)
    }

    public void Initialize(Animator animator, float speed, WeaponCloseRange weaponCloseRange, WeaponLongRange weaponLongRange, Vector3 playerPosition, Vector2 playerDirection, Vector3 weaponPrefabScale, float weaponAttackRange)
    {
        _controller.Initialize(animator, speed, weaponCloseRange, weaponLongRange, playerPosition, playerDirection, weaponPrefabScale, weaponAttackRange);
    }

    public void LocomoteInUpdate(Vector2 locomotionDirection) //сейчас архитектура такова, что это происходит в Update из-за привязки к инпут контроллеру - надо отвязать вызовы от инпут контроллера и вызывать это в FixedUpdate
    {
        _controller.Locomote(transform, _renderAndSkeletonPoint, locomotionDirection);
    }

    public void RunInUpdate(Vector2 locomotionDirection) //переписать, ибо это дубляж механики Locomotion
    {
        _controller.Run(transform, _renderAndSkeletonPoint, locomotionDirection);
    }

    public void AttackCloseRange(Vector2 playerDirection)
    {
        _controller.AttackCloseRange(transform.position, playerDirection);
    }

    public void AttackLongRange(Vector2 playerDirection)
    {
        _controller.AttackLongRange(transform.position, playerDirection);
    }

    public void ApplyDamage(float damage)
    {
        //_playerController.PlayerHealthController...
    }
}
