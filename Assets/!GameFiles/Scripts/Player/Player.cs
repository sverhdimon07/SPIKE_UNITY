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

    public void Initialize(float health, float locomotionSpeed, float runningSpeed, Vector2 direction, WeaponCloseRange weaponCloseRange, WeaponLongRange weaponLongRange)
    {
        _controller.Initialize(GetComponent<Animator>(), health, locomotionSpeed, runningSpeed, transform.position, direction, weaponCloseRange, weaponLongRange);
    }

    public void TakeDamage(float damage)
    {
        _controller.TakeDamage(damage);
    }

    public void LocomoteInUpdate(Vector2 locomotionDirection) //сейчас архитектура такова, что это происходит в Update из-за привязки к инпут контроллеру - надо отвязать вызовы от инпут контроллера и вызывать это в FixedUpdate
    {
        _controller.Locomote(transform, _renderAndSkeletonPoint, locomotionDirection);
    }

    public void RunInUpdate(Vector2 locomotionDirection) //переписать, ибо это дубляж механики Locomotion
    {
        _controller.Run(transform, _renderAndSkeletonPoint, locomotionDirection);
    }

    public void AttackCloseRange(Vector2 direction)
    {
        _controller.AttackCloseRange(transform.position, direction);
    }

    public void AttackLongRange(Vector2 direction)
    {
        _controller.AttackLongRange(transform.position, direction);
    }
}
