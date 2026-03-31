using System;
using UnityEngine;

public sealed class BootstrapTestScene : Bootstrap //система 3х этапов (по итогу 1 обязательный, ибо Create не нужен отдельный метод И создавать самого себя НЕЛЬЗЯ, а Launch вообще по идее нигде не нужен, ибо если у нас есть предметные методы внутри класса, то их и будем запускать), так как добавились OnEnable и OnDisable) - Создание (важно что за создание самого себя ИЛИ свое время жизни класс отвечать не должен, он всегда создается снаружи), Инициализация (инициализация себя это про создание своих внутренних элементов (или поиск их на сцене) И их последующую инициализацию), Запуск
{
    [Header("Weapons")] //должна быть реализована система, которая будет учитывать выбор геймдизайнера - не поставил оружие именно игроку, он заспавнится без оружия - то есть у нас тут в любом случае будет 3 графы (оружие на сцене, оружие игрока, оружие персонажа)
    [Header("PlayerWeapons")] //Я СДЕЛАЛ ТАК, ЧТОБЫ МОЖНО БЫЛО ИНИТИТЬ ОТСЮДА ДАТУ ЛЮБОГО ТИПА ОРУЖИЯ (ТО ЕСТЬ, ЛЮБОЙ СУЩНОСТИ ОРУЖИЯ), но по идее же у каждого оружия уже изначально есть какая-то дата, но при этом она и не должна быть прямо в скрипте самого оружия проиничена, она должна быть сериализована и закинута в префаб - но тут в любом случае нужно придумать методологию создания скриптов для оружия
    [SerializeField] private WeaponCloseRange _playerWeaponCloseRange;
    [SerializeField] private WeaponData _playerWeaponCloseRangeData; //ПРОВЕРКА НА ОТСУТСТВИЕ ПАРАМЕТРОВ В ИНСПЕКТОРЕ (КАК ЭТО СДЕЛАНО СО ЗДОРОВЬЕМ (ИНКАПСУЛЯЦИЯ))
    [SerializeField] private WeaponLongRange _playerWeaponLongRange;
    [SerializeField] private WeaponData _playerWeaponLongRangeData;
    [Header("CharacterWeapons")] // не придумал, как реализовать правильно (пока у каждого персонажа ровно по одному оружию - все вроде норм, но при расширении - уже проблема, поэтому скорее всего надо отдельно каждого персонажа инитить)
    [SerializeField] private WeaponCloseRange _characterWeaponCloseRange;
    [SerializeField] private WeaponData _characterWeaponCloseRangeData; //ПРОВЕРКА НА ОТСУТСТВИЕ ПАРАМЕТРОВ В ИНСПЕКТОРЕ (КАК ЭТО СДЕЛАНО СО ЗДОРОВЬЕМ (ИНКАПСУЛЯЦИЯ))
    [SerializeField] private WeaponLongRange _characterWeaponLongRange;
    [SerializeField] private WeaponData _characterWeaponLongRangeData;
    //[SerializeField] private Weapon[] weaponsCharacter;
    //[Header("WeaponsOnScene")]
    //[SerializeField] private Weapon[] weaponsOnScene;

    [Header("Player")]
    [Range(1f, 100f)]
    [SerializeField] private float _playerHealth = 100f;
    [Range(1f, 10f)] //это не инкапсуляция, нужно сделать инкапсуляцию на уровне низкоуровневых типов
    [SerializeField] private float _playerLocomotionSpeed = 2.5f;
    [Range(2f, 10f)]
    [SerializeField] private float _playerRunningSpeed = 6.25f;

    [Header("Character")] //ПОКА ОДИН
    [Range(1f, 100f)]
    [SerializeField] private float _characterHealth = 100f;
    [Range(1f, 10f)]
    [SerializeField] private float _characterLocomotionSpeed = 2.5f;
    [Range(2f, 10f)]
    [SerializeField] private float _characterRunningSpeed = 6.25f;

    private ScenePausing _scenePausing;

    private SceneLoading _sceneLoading;

    private GameplayMenuUI _gameplayMenuUI;

    private InputController _inputController;

    private Player _player;

    private Character[] _characters;

    //private readonly Weapon[] weaponsPlayer = new Weapon[2]; //Нужно защитить массив от изменения элементов или подмены, не помню (видик Сакутина)

    private void Awake() //можно сделать абстрактным в родительском классе, типо чтобы у нас был контракт на обязательность реализации, но хз, насколько это правильно и насколько правильно оставлять этот метод публичным
    {
        Initialize();
    }

    private void OnEnable() 
    {
        if (_player == null) //Приятно для других программистов, но насколько это прямо таки нужно? (При этом при отсутствии игрока на сцене бесконечный поток ошибок все равно есть, ибо я не вызывал события в InputController через вопросительный знак)
        {
            return;
        }

        _gameplayMenuUI.ContinueButton.onClick.AddListener(_scenePausing.PauseOrResume);
        _gameplayMenuUI.ContinueButton.onClick.AddListener(_gameplayMenuUI.OpenOrClose);
        _gameplayMenuUI.ExitButton.onClick.AddListener(_sceneLoading.LoadMainMenuScene);
        _inputController.LocomotionDirectionDirected += _player.LocomoteWithinFrame;
        _inputController.RunningButtonHolded += _player.RunWithinFrame;
        _inputController.AttackCloseRangeButtonPressed += _player.AttackCloseRange;
        _inputController.AttackLongRangeButtonPressed += _player.AttackLongRange;
        _inputController.OpeningGameplayeMenuButtonPressed += _scenePausing.PauseOrResume;
        _inputController.OpeningGameplayeMenuButtonPressed += _gameplayMenuUI.OpenOrClose;
        _player.Died += _sceneLoading.LoadTestScene;

        foreach (Character character in _characters)
        {
            character.Died += delegate () { Destroy(character); }; 
        }
    }

    private void OnDisable()
    {
        if (_player == null) //Приятно для других программистов, но насколько это прямо таки нужно?
        {
            return;
        }

        _gameplayMenuUI.ContinueButton.onClick.RemoveListener(_scenePausing.PauseOrResume);
        _gameplayMenuUI.ContinueButton.onClick.RemoveListener(_gameplayMenuUI.OpenOrClose);
        _gameplayMenuUI.ExitButton.onClick.RemoveListener(_sceneLoading.LoadMainMenuScene);
        _inputController.LocomotionDirectionDirected -= _player.LocomoteWithinFrame;
        _inputController.RunningButtonHolded -= _player.RunWithinFrame;
        _inputController.AttackCloseRangeButtonPressed -= _player.AttackCloseRange;
        _inputController.AttackLongRangeButtonPressed -= _player.AttackLongRange;
        _inputController.OpeningGameplayeMenuButtonPressed -= _scenePausing.PauseOrResume;
        _inputController.OpeningGameplayeMenuButtonPressed -= _gameplayMenuUI.OpenOrClose;
        _player.Died += _sceneLoading.LoadTestScene;

        foreach (Character character in _characters)
        {
            character.Died -= delegate () { Destroy(character); };
        }
    }

    public override void Initialize() //именно в этом классе не так важно использовать такой метод, чтобы в него потом вкладывать дочерние методы. Я бы вообще так не делал, ибо не разберусь потом в большом потоке входных данных, лучше упразднить этот метод и вызывать дочерние
    {
        _scenePausing = new ScenePausing();
        _sceneLoading = new SceneLoading();
        _gameplayMenuUI = FindAnyObjectByType<GameplayMenuUI>();
        _inputController = GetComponent<InputController>(); //_inputController.Initialize(); //он инитится сам в себе, наверное плохо, но ничего сделать не могу

        _gameplayMenuUI.Initialize();
        _inputController.Initialize();
        InitializePlayerWeapons();
        InitializeCharacterWeapons();
        InitializeRestSceneWeapons();
        InitializePlayer(); //хз, как под другому, но даже если они запускаются в Awake - OnEnable запускается раньше. Мб если сериализировать эти поля, то все будет норм
        InitializeCharacter();
    }

    private void InitializePlayer() //пока что я делаю поля с игроком и оружием, возможно это излишне и можно создавать сущности в локальных переменных, НО Я НЕ ДУМАЮ ТАК (но при этом дочерний монобех PlayerInputController у Player создается в локальной переменной, так как он нам тут не нужен, этот класс не имеет такой ответственности)
    {
        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);

        if (players.Length < 1)
        {
            throw new Exception("На сцене должен присутствовать минимум 1 игрок");
        }
        else if (players.Length > 1)
        {
            throw new Exception("На сцене должен присутствовать только 1 игрок");
        }
        _player = players[0];

        _player.Initialize(_playerHealth, _playerLocomotionSpeed, _playerRunningSpeed, _playerWeaponCloseRange, _playerWeaponLongRange); //пока в DI даем зависимость так (про speed = 2.5f вручную, new PlayerAttackCloseRange()) - типо имитируем прилет инфы с сервера/GameController'a
        //в ините игрока лютая дичь в конечных параметрах - да вообще все эти параметры надо как-то через сериализованные поля прогидывать (это настраивание среды для геймдизайнера)
    }

    private void InitializeCharacter() //ПЕРЕДЕЛАЛ ЛОГИКУ ДЛЯ БОЛЬШЕЙ ОРИЕНТИРОВАННОСТИ ПОД ГЕЙМДИЗАЙНЕРА, НО ИНИТ ПРОИСХОДИТ НЕПРАВИЛЬНО (надо понять, можно ли как-то искать объекты в объектной иерархии на сцене)
    {
        _characters = FindObjectsByType<Character>(FindObjectsSortMode.None);

        foreach (Character character in _characters)
        {
            character.Initialize(_characterHealth, _characterLocomotionSpeed, _characterRunningSpeed, _characterWeaponCloseRange, _characterWeaponLongRange);
        }
    }

    private void InitializePlayerWeapons()
    {
        _playerWeaponCloseRange.Initialize(_playerWeaponCloseRangeData);
        _playerWeaponLongRange.Initialize(_playerWeaponLongRangeData);
    }

    private void InitializeCharacterWeapons()
    {
        _characterWeaponCloseRange.Initialize(_characterWeaponCloseRangeData);
        _characterWeaponLongRange.Initialize(_characterWeaponLongRangeData);
    }

    private void InitializeRestSceneWeapons()
    {
        //
    }
}

/*ENEMY ATTACK
 using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private Collider rangeCollider;

    private readonly int damageValue = 10;

    private bool isActive;

    private void Awake()
    {
        rangeCollider = GetComponent<Collider>();

        rangeCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider _collider)
    {
        if (_collider.gameObject.GetComponent<Player>() == false)
        {
            return;
        }
        if (isActive == true)
        {
            return;
        }
        isActive = true;
    }

    private void OnTriggerExit(Collider _collider)
    {
        if (_collider.gameObject.GetComponent<Player>() == false)
        {
            return;
        }
        if (isActive == false)
        {
            return;
        }
        isActive = false;
    }

    public void ExecuteAttack()
    {
        if (isActive == true)
        {
            AIEventBus.OnAttackExecuted(damageValue);
        }
    }
}
 */
/*ENEMY VISUAL CALIBRATION
using System;
using UnityEngine;

public class EnemyVisualCalibration : MonoBehaviour
{
    [SerializeField] private Transform characterTransform;

    private Transform enemyTransform;

    private Action firstCallback;

    private void Awake()
    {
        enemyTransform = gameObject.transform;

        DeinitFirstCallback();
    }

    private void Update()
    {
        firstCallback?.Invoke();
    }

    private void OnEnable()
    {
        AIEventBus.ChaseStateEntered += InitFirstCallback;
    }

    private void OnDisable()
    {
        AIEventBus.ChaseStateEntered -= InitFirstCallback;
    }

    private void MakeEnemyToLookAtCharacter()
    {
        Vector3 enemyLookAtDirection = new Vector3(characterTransform.position.x, enemyTransform.position.y, characterTransform.position.z);

        enemyTransform.LookAt(enemyLookAtDirection);
    }

    private void InitFirstCallback()
    {
        firstCallback = MakeEnemyToLookAtCharacter;
    }

    private void DeinitFirstCallback()
    {
        firstCallback = null;
    }
}
*/
/*AI STATE MANAGER
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(AIDestinationSetter))]
public class AIStateManager : MonoBehaviour
{
    [SerializeField] private Transform characterTransform;
    [SerializeField] private Transform fightDestinationTransform;

    public static readonly AIPatrolState PatrolState = new AIPatrolState();
    public static readonly AIChaseState ChaseState = new AIChaseState();
    public static readonly AIAttackState AttackState = new AIAttackState();
    public static readonly AIDefenceState DefenceState = new AIDefenceState();

    private AIDestinationSetter _aiDestinationSetter;
    private AILerp _aiLerp;

    private Transform enemyTransform;

    private bool isAttackDistanceReached;

    private readonly float minDistanceBetweenCharacterAndEnemy = 3f;
    private readonly float distanceBetweenCharacterAndEnemyDivisionPoint = 0.01f;

    public static AIBaseState CurrentState { get; private set; }
    public static AIBaseState LastState { get; private set; }

    public bool IsAttackDistanceReached
    {
        get
        {
            return isAttackDistanceReached;
        }
        private set
        {
            if (isAttackDistanceReached != value)
            {
                isAttackDistanceReached = value;

                if (isAttackDistanceReached == true)
                {
                    if (CurrentState == AttackState)
                    {
                        return;
                    }
                    InvokeFirstCallback();
                }
                else
                {
                    if (LastState == PatrolState)
                    {
                        return;
                    }
                    if (CurrentState == AttackState)
                    {
                        return;
                    }
                    InvokeSecondCallback();
                }
            }
            else
            {
                if (CurrentState != DefenceState)
                {
                    return;
                }
                if (_aiLerp.enabled == true)
                {
                    InvokeFirstCallback();
                }
                else
                {
                    InvokeSecondCallback();
                }
            }
        }
    }

    private void Awake()
    {
        _aiDestinationSetter = GetComponent<AIDestinationSetter>();
        _aiLerp = GetComponent<AILerp>();

        enemyTransform = gameObject.transform;
    }

    private void Update()
    {
        //print(CurrentState);
        RefreshDestinationPosition();
        CalibrateAILerpActivity();
    }

    private void OnEnable()
    {
        AIEventBus.GameplaySceneEntered += SwitchPatrolState;
        AIEventBus.GameplaySceneEntered += DisableAILerp;
    }

    private void OnDisable()
    {
        AIEventBus.GameplaySceneEntered -= SwitchPatrolState;
        AIEventBus.GameplaySceneEntered -= DisableAILerp;
    }

    public void ExitCurrentState()
    {
        CurrentState.Exit(this);
    }

    public void SwitchPatrolState()
    {
        SwitchState(PatrolState);
    }

    public void SwitchChaseState()
    {
        SwitchState(ChaseState);
    }

    public void SwitchAttackState()
    {
        SwitchState(AttackState);
    }

    public void SwitchDefenceState()
    {
        SwitchState(DefenceState);
    }

    public void EnableAILerp()
    {
        _aiLerp.enabled = true;
    }

    public void DisableAILerp()
    {
        _aiLerp.enabled = false;
    }
*/
/*
private void SetDestination(Transform pointTransform)
{
    _aiDestinationSetter.target = pointTransform;
}*/
/*
    private void SwitchState(AIBaseState nextState)
    {
        LastState = CurrentState;
        CurrentState = nextState;

        CurrentState.Enter(this);
    }

    private void RefreshDestinationPosition()
    {
        fightDestinationTransform.position = Vector3.Lerp(characterTransform.position, enemyTransform.position, distanceBetweenCharacterAndEnemyDivisionPoint);
    }

    private void CalibrateAILerpActivity()
    {
        if (CurrentState == PatrolState)
        {
            return;
        }
        if (Vector3.Distance(characterTransform.position, enemyTransform.position) <= minDistanceBetweenCharacterAndEnemy)
        {
            IsAttackDistanceReached = true;
        }
        else
        {
            IsAttackDistanceReached = false;
        }
    }

    private void InvokeFirstCallback()
    {
        DisableAILerp();
        ExitCurrentState();
    }

    private void InvokeSecondCallback()
    {
        EnableAILerp();
        ExitCurrentState();
    }
}
*/
/*AI BASE STATE
public abstract class AIBaseState
{
    public abstract void Enter(AIStateManager manager);

    public abstract void Exit(AIStateManager manager);
}
*/
/*AI ATTACK STATE
using UnityEngine;

public class AIAttackState : AIBaseState
{
    public override void Enter(AIStateManager manager)
    {
        Debug.Log("0000000000000000000000000");
        AIEventBus.OnAttackStateEntered();

        manager.DisableAILerp();
    }

    public override void Exit(AIStateManager manager)
    {
        if (manager.IsAttackDistanceReached == true)
        {
            //Debug.Log(PlayerAttackHandler.IsActive);
            if (PlayerAttackHandler.IsActive == true)
            {
                manager.SwitchDefenceState();

                return;
            }
            else
            {
                manager.SwitchAttackState();

                return;
            }
        }
        else
        {
            manager.SwitchChaseState();

            manager.EnableAILerp();

            return;
        }
    }
}
*/
/*AI CHASE STATE
using UnityEngine;

public class AIChaseState : AIBaseState
{
    public override void Enter(AIStateManager manager)
    {
        if (AIStateManager.LastState == null)
        {
            return;
        }
        AIEventBus.OnChaseStateEntered();
    }

    public override void Exit(AIStateManager manager)
    {
        manager.SwitchAttackState();
    }
}
*/
/*AI PATROL STATE
public class AIPatrolState : AIBaseState
{
    public override void Enter(AIStateManager manager)
    {
        AIEventBus.OnPatrolStateEntered();
    }

    public override void Exit(AIStateManager manager)
    {
        manager.SwitchChaseState();
    }
}
*/
/*AI DEFENSE STATE
 * using UnityEngine;

public class AIDefenceState : AIBaseState
{
    public override void Enter(AIStateManager manager)
    {
        if (AIStateManager.LastState == AIStateManager.DefenceState)
        {
            return;
        }
        AIEventBus.OnBlockStateEntered();
    }

    public override void Exit(AIStateManager manager)
    {
        if (manager.IsAttackDistanceReached == true)
        {
            if (PlayerAttackHandler.IsActive == false)
            {
                manager.SwitchAttackState();
                Debug.Log("333333333333333333333");
                return;
            }
            else
            {
                manager.SwitchDefenceState();

                return;
            }
        }
        else
        {
            manager.SwitchChaseState();

            manager.EnableAILerp();

            return;
        }
    }
}
*/
/*VIEW EVENT BUS
using System;

public class ViewEventBus
{
    public static event Action<float> JetpackFuelLevelChanged;

    public static event Action<float> PlayerHealthLevelChanged;

    public static void OnJetpackFuelLevelChanged(float currentFuelLevel)
    {
        JetpackFuelLevelChanged?.Invoke(currentFuelLevel);
    }

    public static void OnPlayerHealthLevelChanged(float currentHealthLevel)
    {
        PlayerHealthLevelChanged?.Invoke(currentHealthLevel);
    }
}*/