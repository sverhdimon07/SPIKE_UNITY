using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
public sealed class Player : MonoBehaviour, IAttacker, IDamageable
{
    [SerializeField] private Image _uiBar; //хотел переносить эти поля в бутстрап, НО почему-бы все поля, не отвечающие за геймплейную логику, не хранить именно здесь (ибо бутстрап должен инитить ГЕЙМДИЗАЙНЕРСКИЕ ДАННЫЕ, зачем их мешать с ссылками на вспомогательные классы?)?
    [SerializeField] private Transform _renderAndSkeletonPoint;
    [SerializeField] private Transform _thirdPersonCameraControllerPoint;

    private readonly PlayerController _controller = new PlayerController();

    private bool isRunning; //управляющие фраги - ВРЕМЕННАЯ МЕРА(пока нет FSM);

    public UnityAction DamageTaken; //под расширение (мб замедление времени во время стана делать, и возможно это делается при помощи заморозки сцены)
    public UnityAction Died;

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

    public void Idle() //это нужно, чтобы вернуться в Idle состояния из стана; НОРМАЛЬНАЯ, НО ВРЕМЕННАЯ МЕРА (пока нет FSM);
    {
        _controller.Idle();
    }

    public void TakeDamage(float damage)
    {
        _controller.TakeDamage(damage);
    }

    public void Die()
    {
        _controller.Die();
    }

    public void RotateWithinFrame(Vector2 inputDirection)
    {
        _controller.RotateWithinFrame(_renderAndSkeletonPoint, _thirdPersonCameraControllerPoint, inputDirection);
    }

    public void LocomoteWithinFrame(Vector2 inputLocomotionDirection) //пропал публичный метод для бега, а я хотел дописывать контракты на ходьбу, на бег (НО МБ С FSM ВСЕ НАЛАДИТСЯ)
    {
        _controller.LocomoteWithinFrame(transform, _thirdPersonCameraControllerPoint, inputLocomotionDirection, isRunning);
    }

    public void SwitchLocomotionType() //хотя вот он, своеобразный контракт для бега (ответ на коммент выше)
    {
        if (isRunning == false)
        {
            isRunning = true;
        }
        else if (isRunning == true)
        {
            isRunning = false;
        }
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
