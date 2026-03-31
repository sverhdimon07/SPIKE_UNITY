using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
public /*sealed*/ /*abstract*/ class Character : MonoBehaviour, IAttacker, IDamageable //я бы еще накидал контрактов на неуправляемое перемещение
{
    [SerializeField] private Image _uiBar;
    [SerializeField] private Transform _renderAndSkeletonPoint;
    [SerializeField] private Transform _playerPoint; //НЕНУЖНАЯ ПРИВЯЗКА - УДАЛЮ ПОТОМ (когда будет FSM)

    private readonly CharacterControllerNew _controller = new CharacterControllerNew(); //можно использовать DI, но пока что это излишняя гибкость

    private int counter;

    private bool _isCloseToPlayer;

    public UnityAction DamageTaken; //под расширение (мб замедление времени во время стана делать, и возможно это делается при помощи заморозки сцены)
    public UnityAction Died;

    private void Update() //возможно здесь будем корректировать то, куда смотрит ГГ (но возможно это стоит делать не здесь)
    {
        transform.LookAt(_playerPoint.position);

        if (Vector3.Distance(transform.position, _playerPoint.position) > 1.1f) //МГ
        {
            _isCloseToPlayer = false;

            counter = 0;

            return;
        }
        _isCloseToPlayer = true;

        PlayIdleAnimation();

        if (counter == 0)
        {
            Attack();

            counter += 1;
        }
    }

    private void FixedUpdate()
    {
        if (_isCloseToPlayer == true)
        {
            return;
        }
        LocomoteInFixedUpdate(new Vector2(transform.forward.x, transform.forward.z));
    }

    private void OnEnable()
    {
        _controller.DamageTaken += delegate () { DamageTaken?.Invoke(); };
        _controller.Died += delegate () { Died?.Invoke(); };
    }

    private void OnDisable()
    {
        _controller.DamageTaken -= delegate () { DamageTaken?.Invoke(); };
        _controller.Died -= delegate () { Died?.Invoke(); };
    }

    public void Initialize(float health, float locomotionSpeed, float runningSpeed, WeaponCloseRange weaponCloseRange, WeaponLongRange weaponLongRange)
    {
        _controller.Initialize(_uiBar, GetComponent<Animator>(), health, locomotionSpeed, runningSpeed, transform.position, new Vector2(_renderAndSkeletonPoint.forward.x, _renderAndSkeletonPoint.forward.z), weaponCloseRange, weaponLongRange);
    }

    public void TakeDamage(float damage)
    {
        _controller.TakeDamage(damage);
    }

    public void Die()
    {
        _controller.Die();
    }

    public void PlayIdleAnimation() //ВРЕМЕННАЯ МЕРА (пока нет FSM)
    {
        _controller.PlayIdleAnimation();
    }

    public void LocomoteInFixedUpdate(Vector2 locomotionDirection) //сейчас архитектура такова, что это происходит в Update из-за привязки к инпут контроллеру - надо отвязать вызовы от инпут контроллера и вызывать это в FixedUpdate
    {
        _controller.Locomote(transform, _renderAndSkeletonPoint, locomotionDirection);
    }

    public void RunInFixedUpdate(Vector2 locomotionDirection) //переписать, ибо это дубляж механики Locomotion
    {
        _controller.Run(transform, _renderAndSkeletonPoint, locomotionDirection);
    }

    public void Attack()
    {
        _controller.Attack(transform.position, new Vector2(_renderAndSkeletonPoint.forward.x, _renderAndSkeletonPoint.forward.z));
    }

    /*
    public void AttackCloseRange(Vector2 direction)
    {
        _controller.AttackCloseRange(transform.position, direction);
    }

    public void AttackLongRange(Vector2 direction)
    {
        _controller.AttackLongRange(transform.position, direction);
    }*/
}
