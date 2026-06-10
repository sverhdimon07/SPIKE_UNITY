using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
public abstract class CharacterControllerNew : MonoBehaviour, IDamageable //тут прикол такой, что этот контроллер тоже может реализовывать интерфейсы модели, НО стоит ли реализовывать их и там, и там - или поставить только тут и все?
{
    public Character _model; //не уверен, что так правильно; вообще есть 2 варика - ЛИБО сделать так, как здесь, с публичной для вызова публичных методов моделью и подписками вьюшки здесь, ЛИБО сделать модель приватной И соединять модель и вьюшку напрямую публичными методами. Мне впринципе 2й варик больше нравится;

    public UnityAction Died; //выходы для фабрик или классов более высокого уровня
    public UnityAction<float> DamageTaken;

    //[SerializeField] private Transform _renderAndSkeletonPivot;
    protected Transform _playerPoint; //НЕНУЖНАЯ ПРИВЯЗКА - УДАЛЮ ПОТОМ (когда будет FSM)
    [SerializeField] protected Transform _lookAndLocomotionPoint;

    [Header("View References")]
    [SerializeField] private ParticleSystem _waterEffects;
    [SerializeField] private ParticleSystem _fireEffects;
    [SerializeField] private ParticleSystem _lightningEffects;
    [SerializeField] private ParticleSystem _windEffects;

    [SerializeField] private AudioSource _waterSound;
    [SerializeField] private AudioSource _fireSound;
    [SerializeField] private AudioSource _lightningSound;
    [SerializeField] private AudioSource _windSound;

    public ParticleSystem WaterEffects => _waterEffects;
    public ParticleSystem FireEffects => _fireEffects;
    public ParticleSystem LightningEffects => _lightningEffects;
    public ParticleSystem WindEffects => _windEffects;

    public AudioSource WaterSound => _waterSound;
    public AudioSource FireSound => _fireSound;
    public AudioSource LightningSound => _lightningSound;
    public AudioSource WindSound => _windSound;

    [SerializeField] private Image _healthBar; //хотел переносить эти поля в бутстрап, НО почему-бы все поля, не отвечающие за геймплейную логику, не хранить именно здесь (ибо бутстрап должен инитить ГЕЙМДИЗАЙНЕРСКИЕ ДАННЫЕ, зачем их мешать с ссылками на вспомогательные классы?)?
    //[SerializeField] private Image _weaponLongRangeCooldownBar;
    //[SerializeField] private TMP_Text _deathMessageText;
    [SerializeField] private Animator _animator;
    [SerializeField] protected Transform _gameObjectPivot;
    [SerializeField] protected Transform _renderAndSkeletonPivot;
    //[SerializeField] private Transform _thirdPersonCameraControllerPivot;

    [Header("Model References")]
    [Range(0.1f, 100f)] //так как это не инкапсуляция - надо чекнуть то, искапсулирую ли я эти входные данные на уровне конкретных реализаций (сервисов) внутри; а вообще монобех именно так и инкапсулируется, я просто хз, как работать со скриптбл обджектами, там наверное как-то по-другому нужно будет проверять входные значения, если я захочу, чтобы у игрока максимальное здоровье было всегда не больше 100f
    [SerializeField] private float _maxHealth = 100f;
    [Range(0.1f, 100f)]
    [SerializeField] private float _health = 100f;
    [Range(1f, 10f)]
    [SerializeField] private float _locomotionSpeed = 2.5f;
    [Range(2f, 10f)]
    [SerializeField] private float _runningSpeed = 6.25f;
    //должна быть реализована система, которая будет учитывать выбор геймдизайнера - не поставил оружие именно игроку, он заспавнится без оружия - то есть у нас тут в любом случае будет 3 графы (оружие на сцене, оружие игрока, оружие персонажа)
    //Я СДЕЛАЛ ТАК, ЧТОБЫ МОЖНО БЫЛО ИНИТИТЬ ОТСЮДА ДАТУ ЛЮБОГО ТИПА ОРУЖИЯ (ТО ЕСТЬ, ЛЮБОЙ СУЩНОСТИ ОРУЖИЯ), но по идее же у каждого оружия уже изначально есть какая-то дата, но при этом она и не должна быть прямо в скрипте самого оружия проиничена, она должна быть сериализована и закинута в префаб - но тут в любом случае нужно придумать методологию создания скриптов для оружия
    [SerializeField] private Weapon _firstWeapon; //ПРОВЕРКА НА ОТСУТСТВИЕ ПАРАМЕТРОВ В ИНСПЕКТОРЕ (КАК ЭТО СДЕЛАНО СО ЗДОРОВЬЕМ (ИНКАПСУЛЯЦИЯ))
    [SerializeField] private Weapon _secondWeapon; //поработать с абстракциями и названиями полей получше (уже)

    private CharacterView _view; //вот та разница между моделью и представлением. Просто если бы это было свойством да еще и публичным, то его методами можно было бы пользоваться в классе более высокого уровня

    protected int counter;

    protected bool _isCloseToPlayer;
    
    public Character Model => _model;

    public Transform GameObjectPivot => _gameObjectPivot; //ВРЕМЕННАЯ МЕРА
    //public Transform ThirdPersonCameraControllerPivot => _thirdPersonCameraControllerPivot; //ВРЕМЕННАЯ МЕРА

    private void Awake() //чекнуть конструкторы и деструкторы в монобехах
    {
        _playerPoint = FindAnyObjectByType<PlayerController>().GetComponent<Transform>();

        _view = new CharacterView(new CharacterUI(_healthBar/*, _weaponLongRangeCooldownBar, _deathMessageText*/), new CharacterAnimator(_animator), _gameObjectPivot, _renderAndSkeletonPivot);
        _model = new Character(new CharacterMechanicStateMachine(_model, new CharacterMechanicIdleState()), new CharacterHealthController(new CharacterHealth(_maxHealth, _health)), new CharacterMovementController(new CharacterLocomotion(_locomotionSpeed, _runningSpeed, transform.position), new CharacterRotation()), new CharacterOffenseController(_firstWeapon, _secondWeapon, transform.position, new Vector2(_gameObjectPivot.forward.x, _gameObjectPivot.forward.z)), new CharacterDefenseController()); //тут такой прикол, что любой человек сможет создавать объект этого класса в любой части программы, но как бы и работать он с ним не сможет без верхнеуровнего монобеховского слоя. Тут все норм, я бы только засинглтонил PlayerController и Player (про PlayerView - хз)
    }

    private void OnEnable()
    {
        _model.Idled += _view.PresentIdle;
        _model.HealthController.Health.DamageTaken += _view.PresentDamageTake;
        _model.HealthController.Health.DamageTaken += DamageTaken; //под расширение (мб замедление времени во время стана делать, и возможно это делается при помощи заморозки сцены)
        //CharacterHealth.Died += _view.PresentDeath; //надо дописать где-то вызов на выключение на старте, и включить объект в сцене
        _model.HealthController.Health.Died += OnDeath;
        _model.HealthController.Health.Died += Died;
        _model.MovementController.Locomotion.Locomoted += _view.MoveCharacterModelInLocomotionForm;
        _model.MovementController.Locomotion.Runned += _view.MoveCharacterModelInRunForm;
        _model.MovementController.Rotation.Rotated += _view.TurnCharacterModel;
        _model.OffenseController.FirstAttackType.Attacked += _view.PresentCloseRangeAttack;
        _model.OffenseController.SecondAttackType.Attacked += _view.PresentLongRangeAttack;
    }

    private void OnDisable()
    {
        _model.Idled -= _view.PresentIdle;
        _model.HealthController.Health.DamageTaken -= _view.PresentDamageTake;
        _model.HealthController.Health.DamageTaken -= DamageTaken; //под расширение (мб замедление времени во время стана делать, и возможно это делается при помощи заморозки сцены)
        //CharacterHealth.Died -= _view.PresentDeath; //надо дописать где-то вызов на выключение на старте, и включить объект в сцене
        _model.HealthController.Health.Died -= OnDeath;
        _model.HealthController.Health.Died -= Died;
        _model.MovementController.Locomotion.Locomoted -= _view.MoveCharacterModelInLocomotionForm;
        _model.MovementController.Locomotion.Runned -= _view.MoveCharacterModelInRunForm;
        _model.MovementController.Rotation.Rotated -= _view.TurnCharacterModel;
        _model.OffenseController.FirstAttackType.Attacked -= _view.PresentCloseRangeAttack;
        _model.OffenseController.SecondAttackType.Attacked -= _view.PresentLongRangeAttack;
    }

    public void SetRenderAndSkeletonPivot(Quaternion rotation) //?
    {
        _gameObjectPivot.rotation = rotation;
    }

    public void Idle()
    {
        _model.Idle();
    }

    public void TakeDamage(float damage)
    {
        _model.TakeDamage(damage);
    }

    public void Die()
    {
        _model.Die();
    }

    public void Locomote(/*Transform thirdPersonCameraControllerPivot, */Vector2 inputDirection)
    {
        _model.Locomote(/*thirdPersonCameraControllerPivot, */inputDirection);
    }

    public void Run(/*Transform thirdPersonCameraControllerPivot, */Vector2 inputDirection)
    {
        _model.Run(/*thirdPersonCameraControllerPivot, */inputDirection);
    }

    public void Rotate(/*Transform thirdPersonCameraControllerPivot, */Vector2 inputDirection) //сначала хотел не вставлять сюда этот метод, но пришлось, ибо он нужен был для бутстрапа
    {
        _model.Rotate(/*thirdPersonCameraControllerPivot, */inputDirection);
    }

    public void OnDeath()
    {
        if (gameObject.GetComponent<CharacterControllerNewBoss>())
        {
            if (CharacterControllerNewBoss.FalseIfItIsFirstStage == true)
            {
                CharacterControllerNewBoss.FalseIfItIsFirstStage = false;
            }
            else if (CharacterControllerNewBoss.FalseIfItIsFirstStage == false)
            {
                CharacterControllerNewBoss.FalseIfItIsFirstStage = true;
            }

            Model.HealthController.Health.Heal();

            CharacterControllerNewBoss.PlayerCompleteBossLives += 1;
            if (CharacterControllerNewBoss.PlayerCompleteBossLives >= 3)
            {
                Destroy(gameObject);
                BossDied.Invoke();
            }
            return;
        }

        Destroy(gameObject);
    }

    public static UnityAction BossDied;
}
