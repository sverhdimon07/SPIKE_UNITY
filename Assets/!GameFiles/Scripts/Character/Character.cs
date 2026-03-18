using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public /*abstract*/ class Character : MonoBehaviour, IAttacker, IDamageableCharacter //€ бы еще накидал контрактов на неуправл€емое перемещение
{
    [SerializeField] private Image _uiBar;
    [SerializeField] private Transform _renderAndSkeletonPoint;
    [SerializeField] private Transform _playerPoint; //Ќ≈Ќ”∆Ќјя ѕ–»¬я« ј - ”ƒјЋё ѕќ“ќћ (когда будет FSM)

    private readonly CharacterControllerNew _controller = new CharacterControllerNew(); //можно использовать DI, но пока что это излишн€€ гибкость

    private bool _isCloseToPlayer;

    private void Update() //возможно здесь будем корректировать то, куда смотрит √√ (но возможно это стоит делать не здесь)
    {
        transform.LookAt(_playerPoint.position);

        if (Vector3.Distance(transform.position, _playerPoint.position) > 0.4f)
        {
            _isCloseToPlayer = false;

            return;
        }
        _isCloseToPlayer = true;

        Attack(new Vector2(transform.forward.x, transform.forward.z));
    }

    private void FixedUpdate()
    {
        if (_isCloseToPlayer == true)
        {
            return;
        }
        LocomoteInFixedUpdate(new Vector2(transform.forward.x, transform.forward.z));
    }

    public void Initialize(float health, float locomotionSpeed, float runningSpeed, Vector2 direction, WeaponCloseRange weaponCloseRange, WeaponLongRange weaponLongRange)
    {
        _controller.Initialize(_uiBar, GetComponent<Animator>(), health, locomotionSpeed, runningSpeed, transform.position, direction, weaponCloseRange, weaponLongRange);
    }

    public void TakeDamage(float damage)
    {
        _controller.TakeDamage(damage);
    }

    public void PlayIdleAnimation() //¬–≈ћ≈ЌЌјя ћ≈–ј (пока нет FSM)
    {
        _controller.PlayIdleAnimation();
    }

    public void LocomoteInFixedUpdate(Vector2 locomotionDirection) //сейчас архитектура такова, что это происходит в Update из-за прив€зки к инпут контроллеру - надо отв€зать вызовы от инпут контроллера и вызывать это в FixedUpdate
    {
        _controller.Locomote(transform, _renderAndSkeletonPoint, locomotionDirection);
    }

    public void RunInFixedUpdate(Vector2 locomotionDirection) //переписать, ибо это дубл€ж механики Locomotion
    {
        _controller.Run(transform, _renderAndSkeletonPoint, locomotionDirection);
    }

    public void Attack(Vector2 direction)
    {
        _controller.Attack(transform.position, direction);
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
