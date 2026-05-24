using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
public sealed class PlayerController : MonoBehaviour
{
    [Header("View References")]
    [SerializeField] private Image _healthBar; //хотел переносить эти поля в бутстрап, НО почему-бы все поля, не отвечающие за геймплейную логику, не хранить именно здесь (ибо бутстрап должен инитить ГЕЙМДИЗАЙНЕРСКИЕ ДАННЫЕ, зачем их мешать с ссылками на вспомогательные классы?)?
    [SerializeField] private Image _weaponLongRangeCooldownBar;
    [SerializeField] private TMP_Text _deathMessageText;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _gameObjectPivot;
    [SerializeField] private Transform _renderAndSkeletonPivot;
    [SerializeField] private Transform _thirdPersonCameraControllerPivot;

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
    [SerializeField] private WeaponCloseRange _weaponCloseRange;
    [SerializeField] private WeaponData _weaponCloseRangeData; //ПРОВЕРКА НА ОТСУТСТВИЕ ПАРАМЕТРОВ В ИНСПЕКТОРЕ (КАК ЭТО СДЕЛАНО СО ЗДОРОВЬЕМ (ИНКАПСУЛЯЦИЯ))
    [SerializeField] private WeaponLongRange _weaponLongRange;
    [SerializeField] private WeaponData _weaponLongRangeData;

    public UnityAction DamageTaken; //выходы для фабрик
    public UnityAction Died;

    public UnityAction<Quaternion> Rotated;

    public UnityAction<Vector3> Locomoted;

    private PlayerView View { get; set; } //вот та разница между моделью и представлением. Просто если бы это свойство было публичным, то его методами можно было бы пользоваться в классе более высокого уровня

    public Player Model { get; private set; } //не уверен, что так правильно; вообще есть 2 варика - ЛИБО сделать так, как здесь, с публичной для вызова публичных методов моделью и подписками вьюшки здесь, ЛИБО сделать модель приватной И соединять модель и вьюшку напрямую публичными методами. Мне впринципе 2й варик больше нравится;

    public Transform GameObjectPivot => _gameObjectPivot; //ВРЕМЕННАЯ МЕРА
    public Transform ThirdPersonCameraControllerPivot => _thirdPersonCameraControllerPivot; //ВРЕМЕННАЯ МЕРА

    private void Awake() //чекнуть конструкторы и деструкторы в монобехах
    {
        View = new PlayerView(new PlayerUI(_healthBar, _weaponLongRangeCooldownBar, _deathMessageText), new PlayerAnimator(_animator), _gameObjectPivot, _renderAndSkeletonPivot);
        Model = new Player(new PlayerMechanicStateMachine(Model, new PlayerMechanicIdleState()), new PlayerHealthController(new PlayerHealth(_maxHealth, _health)), new PlayerMovementController(new PlayerLocomotion(_locomotionSpeed, _runningSpeed, transform.position), new PlayerRotation()), new PlayerOffenseController(transform.position, new Vector2(_gameObjectPivot.forward.x, _gameObjectPivot.forward.z), _weaponCloseRange, _weaponLongRange), new PlayerDefenseController()); //тут такой прикол, что любой человек сможет создавать объект этого класса в любой части программы, но как бы и работать он с ним не сможет без верхнеуровнего монобеховского слоя. Тут все норм, я бы только засинглтонил PlayerController и Player (про PlayerView - хз)
    }

    private void Update()
    {
        Model.MechanicStateMachine.State.DoLogicWithinFrame(Model);
    }

    private void OnEnable()
    {
        Model.StartedToIdle += View.PresentIdle;
        Model.DamageTaken += delegate () { View.PresentDamageTake(Model.Health); };
        Model.DamageTaken += DamageTaken;
        Model.Died += View.PresentDeath; //надо дописать где-то вызов на выключение на старте, и включить объект в сцене
        //Model.Locomoted += View.MoveCharacterModel;
        PlayerLocomotion.Locomoted += View.MoveCharacterModelInLocomotionForm;
        PlayerLocomotion.Runned += View.MoveCharacterModelInRunForm;
        //Model.Rotated += View.TurnCharacterModel;
        PlayerRotation.Rotated += View.TurnCharacterModel;
    }

    private void OnDisable()
    {
        Model.StartedToIdle -= View.PresentIdle;
        Model.DamageTaken -= delegate () { View.PresentDamageTake(Model.Health); };
        Model.DamageTaken -= DamageTaken;
        Model.Died -= View.PresentDeath;
        //Model.Locomoted -= View.MoveCharacterModel;
        PlayerLocomotion.Locomoted -= View.MoveCharacterModelInLocomotionForm;
        PlayerLocomotion.Runned -= View.MoveCharacterModelInRunForm;
        //Model.Rotated -= View.TurnCharacterModel;
        PlayerRotation.Rotated -= View.TurnCharacterModel;
    }

    public void Idle()
    {
        View.PresentIdle();
        //Модель пока не пишу, ибо там логики нет
    }

    public void SetRenderAndSkeletonPivot(Quaternion rotation) //?
    {
        _gameObjectPivot.rotation = rotation;
    }
}
