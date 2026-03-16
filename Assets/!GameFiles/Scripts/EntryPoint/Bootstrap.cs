using UnityEngine;

public class Bootstrap : MonoBehaviour //система 3х этапов (по итогу 1 обязательный, ибо Create не нужен отдельный метод И создавать самого себя НЕЛЬЗЯ, а Launch вообще по идее нигде не нужен, ибо если у нас есть предметные методы внутри класса, то их и будем запускать), так как добавились OnEnable и OnDisable) - Создание (важно что за создание самого себя ИЛИ свое время жизни класс отвечать не должен, он всегда создается снаружи), Инициализация (инициализация себя это про создание своих внутренних элементов (или поиск их на сцене) И их последующую инициализацию), Запуск
{
    private GameControllerTestScene _gameController;
    
    private void Awake()
    {
        Initialize();
    }
    
    private void Initialize()
    {
        _gameController = FindAnyObjectByType<GameControllerTestScene>(); //можем создавать его на сцене здесь, но я пока делаю так

        _gameController.Initialize();
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
}
*/
/*PlayerHealthLevelUI
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthLevelUI : MonoBehaviour
{
    [SerializeField] Image bar;

    private Coroutine firstCoroutine;

    private readonly int externalDataScale = 100;

    private readonly float fillSpeed = 0.1f;
    private readonly float delayBetweenSmoothRefreshStages = 0.005f;

    private void OnEnable()
    {
        ViewEventBus.PlayerHealthLevelChanged += Refresh;
    }

    private void OnDisable()
    {
        ViewEventBus.PlayerHealthLevelChanged -= Refresh;
    }

    private void StartSmoothRefreshCoroutine(float fuelLevel)
    {
        firstCoroutine = StartCoroutine(RefreshSmoothly(fuelLevel));
    }

    private void Refresh(float fuelLevel)
    {
        StopAllCoroutines();
        StartSmoothRefreshCoroutine(fuelLevel);
    }

    private IEnumerator RefreshSmoothly(float fuelLevel)
    {
        float barFullness = fuelLevel / externalDataScale;

        while (bar.fillAmount != barFullness)
        {
            bar.fillAmount = Mathf.Lerp(bar.fillAmount, barFullness, fillSpeed);

            yield return new WaitForSeconds(delayBetweenSmoothRefreshStages);
        }
    }
}
*/
/*JetpackFuelLevelUI
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JetpackFuelLevelUI : MonoBehaviour
{
    [SerializeField] Image bar;

    private Coroutine firstCoroutine;

    private readonly int externalDataScale = 200;

    private readonly float fillSpeed = 0.04f;
    private readonly float delayBetweenSmoothRefreshStages = 0.005f;

    private void OnEnable()
    {
        ViewEventBus.JetpackFuelLevelChanged += Refresh;
    }

    private void OnDisable()
    {
        ViewEventBus.JetpackFuelLevelChanged -= Refresh;
    }

    private void StartSmoothRefreshCoroutine(float fuelLevel)
    {
        firstCoroutine = StartCoroutine(RefreshSmoothly(fuelLevel));
    }

    private void Refresh(float fuelLevel)
    {
        StopAllCoroutines();
        StartSmoothRefreshCoroutine(fuelLevel);
    }

    private IEnumerator RefreshSmoothly(float fuelLevel)
    {
        float barFullness = fuelLevel / externalDataScale;

        while (bar.fillAmount != barFullness)
        {
            bar.fillAmount = Mathf.Lerp(bar.fillAmount, barFullness, fillSpeed);

            yield return new WaitForSeconds(delayBetweenSmoothRefreshStages);
        }
    }
}
*/
