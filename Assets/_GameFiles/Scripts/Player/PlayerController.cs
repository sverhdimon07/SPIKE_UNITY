using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
public sealed class PlayerController : MonoBehaviour, IDamageable, IAllRangesAttacker //тут прикол такой, что этот контроллер тоже может реализовывать интерфейсы модели, НО стоит ли реализовывать их и там, и там - или поставить только тут и все?
{
    [Header("View References")]
    [SerializeField] private Image _healthBar; //хотел переносить эти поля в бутстрап, НО почему-бы все поля, не отвечающие за геймплейную логику, не хранить именно здесь (ибо бутстрап должен инитить ГЕЙМДИЗАЙНЕРСКИЕ ДАННЫЕ, зачем их мешать с ссылками на вспомогательные классы?)?
    [SerializeField] private Image _weaponLongRangeCooldownBar;
    [SerializeField] private Image _livesBar;
    [SerializeField] private TMP_Text _deathMessageText;
    [SerializeField] private TMP_Text _counterText;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _gameObjectPivot;
    [SerializeField] private Transform _renderAndSkeletonPivot;
    [SerializeField] private Transform _thirdPersonCameraControllerPivot;
    [SerializeField] private ParticleSystem _closeRangeWeaponEffect;
    [SerializeField] private ParticleSystem _longRangeWeaponEffect;
    [SerializeField] private AudioSource _closeRangeWeaponSound;
    [SerializeField] private AudioSource _longRangeWeaponSound;

    [SerializeField] private Transform _fighter;
    [Min(0.1f)]
    [SerializeField] private float _fighterCameraDistance = 4.8f;
    [SerializeField] private GameObject _hud;

    [Min(0.1f)]
    [SerializeField] private float _flightSpeed = 14f;

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
    [SerializeField] private Weapon _secondWeapon; //кое-чо важное написано в CharacterControllerNew

    private PlayerView _view; //вот та разница между моделью и представлением. Просто если бы это было свойством да еще и публичным, то его методами можно было бы пользоваться в классе более высокого уровня

    private Player _model; //не уверен, что так правильно; вообще есть 2 варика - ЛИБО сделать так, как здесь, с публичной для вызова публичных методов моделью и подписками вьюшки здесь, ЛИБО сделать модель приватной И соединять модель и вьюшку напрямую публичными методами. Мне впринципе 2й варик больше нравится;

    private ScoreController _scoreController;

    private ScenePausing _scenePausing;

    private LoseUI _loseUI;

    //public UnityAction Died; //выходы для фабрик или классов более высокого уровня

    //public UnityAction<float> DamageTaken;

    public Transform GameObjectPivot => _gameObjectPivot; //ВРЕМЕННАЯ МЕРА
    public Transform RenderAndSkeletonPivot => _renderAndSkeletonPivot; //ВРЕМЕННАЯ МЕРА
    public Transform ThirdPersonCameraControllerPivot => _thirdPersonCameraControllerPivot; //ВРЕМЕННАЯ МЕРА

    public event System.Action<bool> FighterFormChanged;

    public bool IsFighter => _model?.IsFighter == true;
    public bool IsFlightLocked { get; private set; }
    public bool IsFalling => Locomotion.IsFalling;
    public float FallSpeed => Locomotion.FallSpeed;
    public Quaternion FormRotation => IsFighter ? _view.FighterRotation : _renderAndSkeletonPivot.rotation;
    private PlayerLocomotion Locomotion => _model.MovementController.Locomotion;

    public Player Model => _model;

    public ScoreController ScoreController => _scoreController;

    public float Health => _model.HealthController.Health.HealthValue;

    private void Awake() //чекнуть конструкторы и деструкторы в монобехах
    {
        _scenePausing = new ScenePausing();

        _scoreController = FindAnyObjectByType<ScoreController>();

        _loseUI = FindAnyObjectByType<LoseUI>();

        _loseUI.Initialize();

        _view = new PlayerView(new PlayerUI(_healthBar, _weaponLongRangeCooldownBar, _livesBar, _deathMessageText, _counterText), new PlayerAnimator(_animator), _gameObjectPivot, _renderAndSkeletonPivot, _closeRangeWeaponEffect, _longRangeWeaponEffect, _closeRangeWeaponSound, _longRangeWeaponSound, _fighter, _hud,
            _thirdPersonCameraControllerPivot.GetComponent<CinemachinePositionComposer>(), _fighterCameraDistance);
        _model = new Player(new PlayerMechanicStateMachine(_model, new PlayerMechanicIdleState()), new PlayerHealthController(new PlayerHealth(_maxHealth, _health)), new PlayerMovementController(new PlayerLocomotion(new EnvironmentAreaOverlapAnalyzer<Collider, PlayerController>(), new Vector2(_renderAndSkeletonPivot.forward.x, _renderAndSkeletonPivot.forward.z), 0.3f, 0.5f, 2.5f, _locomotionSpeed, _runningSpeed, transform.position, GetComponent<CapsuleCollider>(), _flightSpeed), new PlayerRotation()), new PlayerOffenseController(_firstWeapon, _secondWeapon, transform.position, new Vector2(_gameObjectPivot.forward.x, _gameObjectPivot.forward.z)), new PlayerDefenseController()); //тут такой прикол, что любой человек сможет создавать объект этого класса в любой части программы, но как бы и работать он с ним не сможет без верхнеуровнего монобеховского слоя. Тут все норм, я бы только засинглтонил PlayerController и Player (про PlayerView - хз)
        Locomotion.FlightLimiterReached += limiter => limiter.Reach(this);
        Locomotion.AirborneMoved += _view.MoveAirborne;
    }

    private void Start()
    {
        if (!IsFighter)
        {
            Locomotion.RefreshGroundSupport(Locomotion.FallSpeed);
        }
    }

    private void Update()
    {
        _model.MechanicStateUpdate();

        //print(_model.State);
    }

    private void OnEnable()
    {
        Player.Idled += _view.PresentIdle;
        PlayerHealth.DamageTaken += _view.PresentDamageTake;
        //PlayerHealth.DamageTaken += DamageTaken; //под расширение (мб замедление времени во время стана делать, и возможно это делается при помощи заморозки сцены)
        _model.HealthController.Health.Died += _view.PresentDeath; //надо дописать где-то вызов на выключение на старте, и включить объект в сцене
        //_model.HealthController.Health.Died += Died;
        _model.HealthController.Health.LivesChanged += _view.PresentLives;
        _model.HealthController.Health.Blocked += _view.PresentBlock;
        PlayerLocomotion.Locomoted += _view.MoveCharacterModelInLocomotionForm;
        PlayerLocomotion.Runned += _view.MoveCharacterModelInRunForm;
        PlayerRotation.Rotated += _view.TurnCharacterModel;
        _model.OffenseController.FirstAttackType.Attacked += async delegate { await _view.PresentCloseRangeAttack(); };
        _model.OffenseController.SecondAttackType.Attacked += async delegate { await _view.PresentLongRangeAttack(); };
        ScoreController.ScoreIncreased += _view.PresentScoreIncrease;

        _loseUI.MenuButton.onClick.AddListener(SceneLoading.LoadMainMenuScene);
        _loseUI.LoadLevelButton.onClick.AddListener(SceneLoading.LoadLevelScene);

        _model.HealthController.Health.Died += _loseUI.OpenOrClose;
        _model.HealthController.Health.Died += _scenePausing.PauseOrResume;
    }

    private void OnDisable()
    {
        Player.Idled -= _view.PresentIdle;
        PlayerHealth.DamageTaken -= _view.PresentDamageTake;
        //PlayerHealth.DamageTaken -= DamageTaken;
        _model.HealthController.Health.Died -= _view.PresentDeath;
        //_model.HealthController.Health.Died -= Died;
        _model.HealthController.Health.LivesChanged -= _view.PresentLives;
        _model.HealthController.Health.Blocked -= _view.PresentBlock;
        PlayerLocomotion.Locomoted -= _view.MoveCharacterModelInLocomotionForm;
        PlayerLocomotion.Runned -= _view.MoveCharacterModelInRunForm;
        PlayerRotation.Rotated -= _view.TurnCharacterModel;
        _model.OffenseController.FirstAttackType.Attacked -= async delegate { await _view.PresentCloseRangeAttack(); };
        _model.OffenseController.SecondAttackType.Attacked -= async delegate { await _view.PresentLongRangeAttack(); };
        ScoreController.ScoreIncreased -= _view.PresentScoreIncrease;

        _loseUI.MenuButton.onClick.RemoveListener(SceneLoading.LoadMainMenuScene);
        _loseUI.LoadLevelButton.onClick.RemoveListener(SceneLoading.LoadLevelScene);

        _model.HealthController.Health.Died -= _loseUI.OpenOrClose;
        _model.HealthController.Health.Died -= _scenePausing.PauseOrResume;
    }

    public void SetLastPosition(Vector3 lastPosition)
    {
        _model.MovementController.Locomotion.SetLastPosition(lastPosition);
    }

    public void SetRenderAndSkeletonPivot(Quaternion rotation) //?
    {
        _gameObjectPivot.rotation = rotation;
    }

    public void SetHealthValue(float healthValue)
    {
        _model.HealthController.Health.SetHealthValue(healthValue);
    }

    public void ToggleFighterForm()
    {
        if (IsFlightLocked || _fighter == null || _hud == null || Time.timeScale <= 0f)
        {
            return;
        }

        Locomotion.SetLastPosition(_gameObjectPivot.position);
        if (IsFighter)
        {
            ExitFighterForm();
        }
        else
        {
            SetFighterForm(true);
        }
    }

    public void LockFighterForm()
    {
        IsFlightLocked = true;
        if (IsFighter)
        {
            ExitFighterForm();
        }
    }

    private void ExitFighterForm()
    {
        SetFighterForm(false);
        Locomotion.RefreshGroundSupport();
    }

    public void RestoreForm(Vector3 position, Quaternion rotation, bool isFighter, bool isFalling, float fallSpeed, bool isFlightLocked)
    {
        _gameObjectPivot.position = position;
        Locomotion.SetLastPosition(position);
        _view.CancelAttackPresentation();

        IsFlightLocked = isFlightLocked;
        isFighter = isFighter && !IsFlightLocked && _fighter != null && _hud != null;
        if (!IsFighter)
        {
            _model.ResetToHuman();
        }

        SetFighterForm(isFighter);
        if (Quaternion.Dot(rotation, rotation) > 0f)
        {
            _view.TurnCharacterModel(rotation);
        }

        if (!IsFighter)
        {
            Locomotion.RefreshGroundSupport(isFalling ? fallSpeed : 0f);
        }
    }

    private void SetFighterForm(bool isFighter)
    {
        if (IsFighter == isFighter || (isFighter && !_model.TryEnterFighterForm()))
        {
            return;
        }

        _view.SetFighterForm(isFighter, _thirdPersonCameraControllerPivot.forward);
        if (!isFighter)
        {
            _model.Idle();
        }

        FighterFormChanged?.Invoke(isFighter);
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

    public void Locomote(Transform thirdPersonCameraControllerPivot, Vector2 inputDirection, Vector2 renderAndSkeletonPivot)
    {
        _model.Locomote(thirdPersonCameraControllerPivot, inputDirection, renderAndSkeletonPivot);
    }

    public void Run(Transform thirdPersonCameraControllerPivot, Vector2 inputDirection, Vector2 renderAndSkeletonPivot)
    {
        _model.Run(thirdPersonCameraControllerPivot, inputDirection, renderAndSkeletonPivot);
    }

    public void Rotate(Transform thirdPersonCameraControllerPivot, Vector2 inputDirection) //сначала хотел не вставлять сюда этот метод, но пришлось, ибо он нужен был для бутстрапа
    {
        _model.Rotate(thirdPersonCameraControllerPivot, inputDirection);
    }

    public void AttackCloseRange(Vector3 gameObjectPosition, Vector2 gameObjectRotation)
    {
        _model.AttackCloseRange(gameObjectPosition, gameObjectRotation);
    }

    public void AttackLongRange(Vector3 gameObjectPosition, Vector2 gameObjectRotation)
    {
        _model.AttackLongRange(gameObjectPosition, gameObjectRotation);
    }

    public void Block()
    {
        _model.Block(); //ВЕЗДЕ СДЕЛАТЬ ТАК - но нет же, я был не прав, ибо у нас внтури оч сложная логика со стейт машиной, которая лежит внутри и сюда ее вносить - это бред
    }

    public void Unblock()
    {
        _model.HealthController.Health.Unblock(); //ВЕЗДЕ СДЕЛАТЬ ТАК
        _model.Idle();
    }
}
