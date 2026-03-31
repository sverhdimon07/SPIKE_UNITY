using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
public sealed class Player : MonoBehaviour, IAttackerCloseRange, IAttackerLongRange, IDamageable //я бы еще накидал контрактов на управляемое перемещение
{
    [SerializeField] private Image _uiBar; //ПЕРЕНЕСТИ ВСЕ ЭТИ ШТУКИ В БУТСТРАП
    [SerializeField] private Transform _renderAndSkeletonPoint; //ПЕРЕНЕСТИ ВСЕ ЭТИ ШТУКИ В БУТСТРАП
    [SerializeField] private Transform _thirdPersonCameraControllerPoint; //ПЕРЕНЕСТИ ВСЕ ЭТИ ШТУКИ В БУТСТРАП

    private readonly PlayerController _controller = new PlayerController(); //можно использовать DI, но пока что это излишняя гибкость

    public UnityAction DamageTaken; //под расширение (мб замедление времени во время стана делать, и возможно это делается при помощи заморозки сцены)
    public UnityAction Died;

    private void Update()
    {
        //возможно здесь будем корректировать то, куда смотрит ГГ (но возможно это стоит делать не здесь)
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

    public void PlayIdleAnimation() //это нужно, чтобы вернуться в Idle состояния из стана; ВРЕМЕННАЯ МЕРА (пока нет FSM);
    {
        _controller.PlayIdleAnimation();
    }

    public void LocomoteWithinFrame(Vector2 locomotionDirection) //сейчас архитектура такова, что это происходит в Update из-за привязки к инпут контроллеру - надо отвязать вызовы от инпут контроллера и вызывать это в FixedUpdate
    {
        _controller.LocomoteWithinFrame(transform, _renderAndSkeletonPoint, locomotionDirection, _thirdPersonCameraControllerPoint);
    }

    public void RunWithinFrame(Vector2 locomotionDirection) //переписать, ибо это дубляж механики Locomotion
    {
        _controller.RunWithinFrame(transform, _renderAndSkeletonPoint, locomotionDirection, _thirdPersonCameraControllerPoint);
    }

    public void AttackCloseRange()
    {
        _controller.AttackCloseRange(transform.position, new Vector2(_renderAndSkeletonPoint.forward.x, _renderAndSkeletonPoint.forward.z));
    }

    public void AttackLongRange()
    {
        _controller.AttackLongRange(transform.position, new Vector2(_renderAndSkeletonPoint.forward.x, _renderAndSkeletonPoint.forward.z));
    }
}
