using UnityEngine;
using UnityEngine.UI;

public class CharacterControllerNewBoss : CharacterControllerNew, IAllRangesAttacker
{
    private const float PhaseTwoCombatDuration = 10f;
    private const float PhaseTwoRecoveryDuration = 3f;
    private const float CollisionSkin = 0.02f;
    private static readonly int IdleAnimation = Animator.StringToHash("Base Layer.Idle");
    private static readonly int CloseRangeAttackTrigger = Animator.StringToHash("AttackCloseRange");
    private static readonly int LongRangeAttackTrigger = Animator.StringToHash("AttackLongRange");

    [SerializeField] private Image _livesBar;

    [SerializeField] private WinUI _winUI;
    private ScenePausing _scenePausing;

    public WeaponType WeaponType;
    public AttackType AttackType;

    [Header("Combat Settings")]
    [SerializeField] private float meleeRange = 1.1f;
    [SerializeField] private float attackCooldown = 4f;

    [Header("Ranged Behavior (Phase 1)")]
    [SerializeField] private float timeToRangedAttack = 5f;
    [SerializeField] private float chaseDuration = 10f;

    [Header("Block Settings")]
    [SerializeField] private float blockDuration = 3f;
    [SerializeField] private float blockCooldown = 6f;
    [SerializeField] private float blockChance = 0.25f;

    // --- Параметры фаз ---
    [Header("Phase Settings")]
    [SerializeField] private int globalLives = 6;
    [SerializeField] private GameObject _limiters;
    [SerializeField] private ParticleSystem _limitersParticle;
    [SerializeField, Min(0.1f)] private float phaseTwoTeleportDistance = 2f;
    [SerializeField] private float phaseOneAttackCooldown = 1.5f;
    [SerializeField] private float phaseTwoAttackCooldown = 0.8f;
    [SerializeField] private float initialSpawnDistance = 10f; // дистанция от игрока при первом появлении

    private int _currentGlobalLife;
    private bool _isPhaseTwoActive;
    private bool _isRecoveringInPhaseTwo;
    private double _phaseTwoStateEndsAt;

    private Animator _bossAnimator;
    private CapsuleCollider _bossCapsule;
    private CapsuleCollider _playerCapsule;
    private Limiters _phaseTwoLimiters;
    private Quaternion _limitersRotation;
    private Vector3 _limitersScale;
    private readonly RaycastHit[] _phaseTwoMovementHits = new RaycastHit[8];

    private float _lastAttackTime;
    private float _timeSinceFar;
    private bool _hasDoneRangedAttack;
    private bool _isChasing;
    private float _chaseTimer;

    private bool _isBlocking;
    private float _blockTimer;
    private float _timeSinceLastBlock;

    private bool _wasInMeleeRange = false;

    public override void Awake()
    {
        base.Awake();

        if (WeaponType == WeaponType.First)
        {
            _firstSword.SetActive(true);
            _secondSword.SetActive(false);
            _firstGun.SetActive(true);
            _secondGun.SetActive(false);
        }
        else if (WeaponType == WeaponType.Second)
        {
            _firstSword.SetActive(false);
            _secondSword.SetActive(true);
            _firstGun.SetActive(false);
            _secondGun.SetActive(true);
        }

        _scenePausing = new ScenePausing();
        _winUI.MenuButton.onClick.AddListener(SceneLoading.LoadMainMenuScene);

        Model.HealthController.Health.Died += OnBossDeath;

        _currentGlobalLife = 0;
        _isPhaseTwoActive = false;
        attackCooldown = phaseOneAttackCooldown;
        _bossAnimator = GetComponent<Animator>();
        _bossCapsule = GetComponent<CapsuleCollider>();
        _playerCapsule = _playerPoint.GetComponent<CapsuleCollider>();

        if (_limiters != null)
        {
            _phaseTwoLimiters = _limiters.GetComponent<Limiters>();
            _limitersRotation = _limiters.transform.rotation;
            _limitersScale = _limiters.transform.lossyScale;
            _limiters.SetActive(false);
        }
    }

    public override void Start()
    {
        base.Start();
        // Отодвигаем босса от игрока при старте
        SetInitialPosition();
    }

    private void SetInitialPosition()
    {
        if (_playerPoint == null) return;
        Vector3 randomDirection = Random.insideUnitSphere;
        randomDirection.y = 0;
        randomDirection.Normalize();
        Vector3 targetPosition = _playerPoint.position + randomDirection * initialSpawnDistance;
        targetPosition.y = transform.position.y;
        transform.position = targetPosition;
    }

    public override void Update()
    {
        base.Update();

        if (_isPhaseTwoActive)
        {
            HandlePhaseTwoBehaviour();
            return;
        }

        _gameObjectPivot.LookAt(_playerPoint);
        float distance = Vector3.Distance(transform.position, _playerPoint.position);

        if (distance <= meleeRange)
            HandleMelee(distance);
        else
            HandleRanged(distance);
    }

    private void HandlePhaseTwoBehaviour()
    {
        if (Time.deltaTime <= 0f)
            return;

        if (Time.timeAsDouble >= _phaseTwoStateEndsAt)
        {
            _isRecoveringInPhaseTwo = !_isRecoveringInPhaseTwo;
            _phaseTwoStateEndsAt = Time.timeAsDouble +
                (_isRecoveringInPhaseTwo ? PhaseTwoRecoveryDuration : PhaseTwoCombatDuration);

            if (_isRecoveringInPhaseTwo)
            {
                Unblock();
                _bossAnimator.ResetTrigger(CloseRangeAttackTrigger);
                _bossAnimator.ResetTrigger(LongRangeAttackTrigger);
                _bossAnimator.CrossFadeInFixedTime(IdleAnimation, 0.1f);
            }
        }

        if (_isRecoveringInPhaseTwo)
            return;

        Vector3 toPlayer = _playerPoint.position - transform.position;
        toPlayer.y = 0f;
        float distance = toPlayer.magnitude;
        if (distance > 0.001f)
            _gameObjectPivot.rotation = Quaternion.LookRotation(toPlayer);

        if (distance > meleeRange)
        {
            ChaseInPhaseTwo(toPlayer / distance, distance);
            return;
        }

        if (Model.State is CharacterMechanicLocomotionState)
            Idle();

        if (Time.time - _lastAttackTime < attackCooldown)
            return;

        Idle();
        AttackCloseRange(_gameObjectPivot.position, new Vector2(_gameObjectPivot.forward.x, _gameObjectPivot.forward.z));
        _lastAttackTime = Time.time;
    }

    private void ChaseInPhaseTwo(Vector3 direction, float distanceToPlayer)
    {
        float fullStep = Model.MovementController.Locomotion.LocomotionSpeed * Time.deltaTime;
        float step = Mathf.Min(fullStep, distanceToPlayer - meleeRange * 0.9f);
        GetBossCapsule(out Vector3 bottom, out Vector3 top, out float radius);
        RaycastHit[] hits = _phaseTwoMovementHits;
        int count = Physics.CapsuleCastNonAlloc(bottom, top, radius, direction, hits,
            step + CollisionSkin, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        if (count == hits.Length)
        {
            hits = Physics.CapsuleCastAll(bottom, top, radius, direction,
                step + CollisionSkin, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            count = hits.Length;
        }

        for (int i = 0; i < count; i++)
        {
            RaycastHit hit = hits[i];
            if (!hit.collider.transform.IsChildOf(transform) && Vector3.Dot(hit.normal, direction) < -0.001f)
                step = Mathf.Min(step, hit.distance - CollisionSkin);
        }

        if (!(Model.State is CharacterMechanicIdleState || Model.State is CharacterMechanicLocomotionState))
            Idle();

        if (step <= 0f)
        {
            if (Model.State is CharacterMechanicLocomotionState)
                Idle();
            return;
        }

        Vector3 input = direction * (step / fullStep);
        Locomote(new Vector2(input.x, input.z));
    }

    // --- ПЕРВАЯ ФАЗА: ближняя зона ---
    private void HandleMelee(float distance)
    {
        _timeSinceFar = 0f;
        _hasDoneRangedAttack = false;
        _isChasing = false;
        _chaseTimer = 0f;

        if (!_wasInMeleeRange)
        {
            _isBlocking = true;
            _blockTimer = 4f;
            Block();
            _timeSinceLastBlock = Time.time + Random.Range(8f, 15f);
            _wasInMeleeRange = true;
        }

        UpdateBlock(distance);

        if (_isBlocking)
            return;

        if (Time.time - _lastAttackTime >= attackCooldown)
        {
            AttackCloseRange(_gameObjectPivot.position, new Vector2(_gameObjectPivot.forward.x, _gameObjectPivot.forward.z));
            _lastAttackTime = Time.time;
        }
    }

    // --- ПЕРВАЯ ФАЗА: дальняя зона ---
    private void HandleRanged(float distance)
    {
        _wasInMeleeRange = false;

        if (_isBlocking)
        {
            Unblock();
            _isBlocking = false;
            _blockTimer = 0f;
        }

        if (_isChasing)
        {
            _chaseTimer += Time.deltaTime;
            Locomote(new Vector2(_gameObjectPivot.forward.x, _gameObjectPivot.forward.z));

            if (distance <= meleeRange)
            {
                _isChasing = false;
                _chaseTimer = 0f;
                _timeSinceFar = 0f;
                _hasDoneRangedAttack = false;
            }
            else if (_chaseTimer >= chaseDuration)
            {
                PerformRangedAttack();
                _chaseTimer = 0f;
            }
        }
        else
        {
            _timeSinceFar += Time.deltaTime;

            if (_timeSinceFar < timeToRangedAttack)
            {
                Locomote(new Vector2(_gameObjectPivot.forward.x, _gameObjectPivot.forward.z));
            }
            else if (!_hasDoneRangedAttack)
            {
                PerformRangedAttack();
                _hasDoneRangedAttack = true;
                _isChasing = true;
                _chaseTimer = 0f;
            }
            else
            {
                Locomote(new Vector2(_gameObjectPivot.forward.x, _gameObjectPivot.forward.z));
            }
        }
    }

    private void UpdateBlock(float distance)
    {
        if (distance > meleeRange)
            return;

        if (_isBlocking)
        {
            _blockTimer -= Time.deltaTime;
            if (_blockTimer <= 0f)
            {
                Unblock();
                _isBlocking = false;
                _blockTimer = 0f;
            }
            return;
        }

        if (Time.time - _timeSinceLastBlock >= blockCooldown &&
            Random.value < blockChance)
        {
            _isBlocking = true;
            _blockTimer = blockDuration;
            _timeSinceLastBlock = Time.time;
            Block();
        }
    }

    private void PerformRangedAttack()
    {
        if (Time.time - _lastAttackTime >= attackCooldown)
        {
            AttackLongRange(_gameObjectPivot.position, new Vector2(_gameObjectPivot.forward.x, _gameObjectPivot.forward.z));
            _lastAttackTime = Time.time;
        }
    }

    private void OnBossDeath()
    {
        _currentGlobalLife++;

        if (_currentGlobalLife == 2)
        {
            _scoreController.IncreaseScore();

            _livesBar.fillAmount = 0.66f;
        }
        else if (_currentGlobalLife == 4)
        {
            _scoreController.IncreaseScore();

            _livesBar.fillAmount = 0.33f;
        }
        else if (_currentGlobalLife == 6)
        {
            _scoreController.IncreaseScore();

            _livesBar.fillAmount = 0f;
        }

        if (_currentGlobalLife < globalLives)
        {
            if (_isPhaseTwoActive)
                ReturnToPhaseOne();
            else
                ActivatePhaseTwo();

            SetHealthValue(Model.HealthController.Health.MaxHealthValue);

            if (_isBlocking)
            {
                Unblock();
                _isBlocking = false;
                _blockTimer = 0f;
            }
        }
        else
        {
            _winUI.OpenOrClose();
            _scenePausing.PauseOrResume();
        }
    }

    private void ActivatePhaseTwo()
    {
        _isPhaseTwoActive = true;
        _isRecoveringInPhaseTwo = false;
        _phaseTwoStateEndsAt = Time.timeAsDouble + PhaseTwoCombatDuration;
        attackCooldown = phaseTwoAttackCooldown;

        Unblock();
        _isBlocking = false;
        _blockTimer = 0f;

        TeleportBossToPlayer();

        if (_limiters != null)
        {
            _limiters.transform.SetParent(null);
            _limiters.transform.rotation = _limitersRotation;
            _limiters.transform.localScale = _limitersScale;
            Vector3 center = (transform.position + _playerPoint.position) * 0.5f;
            center.y = Mathf.Min(transform.position.y, _playerPoint.position.y);
            float requiredRadius = Mathf.Max(GetHorizontalReach(_bossCapsule, center),
                GetHorizontalReach(_playerCapsule, center)) + 0.25f;
            if (_phaseTwoLimiters != null)
                _phaseTwoLimiters.PlaceAround(center, requiredRadius);

            _limiters.SetActive(true);
            Physics.SyncTransforms();
            if (_limitersParticle != null)
                _limitersParticle.Play();
        }

        _timeSinceFar = 0f;
        _hasDoneRangedAttack = false;
        _isChasing = false;
        _chaseTimer = 0f;
        _lastAttackTime = -attackCooldown;
        _wasInMeleeRange = false;
    }

    private void ReturnToPhaseOne()
    {
        _isPhaseTwoActive = false;
        attackCooldown = phaseOneAttackCooldown;

        if (_limiters != null)
        {
            _limiters.SetActive(false);
            _limiters.transform.SetParent(transform);
            if (_limitersParticle != null)
                _limitersParticle.Stop();
        }

        _timeSinceFar = 0f;
        _hasDoneRangedAttack = false;
        _isChasing = false;
        _chaseTimer = 0f;
        _lastAttackTime = -attackCooldown;
        _wasInMeleeRange = false;
    }

    private void TeleportBossToPlayer()
    {
        Vector3 direction = transform.position - _playerPoint.position;
        direction.y = 0f;
        direction = direction.sqrMagnitude > 0.01f ? direction.normalized : Vector3.forward;
        _gameObjectPivot.rotation = Quaternion.LookRotation(-direction);
        Physics.SyncTransforms();

        float minimumDistance = GetCapsuleRadius(_bossCapsule) + GetCapsuleRadius(_playerCapsule) + 0.15f;
        float preferredDistance = Mathf.Max(phaseTwoTeleportDistance, minimumDistance);
        if (TryFindPhaseTwoPosition(direction, preferredDistance, out Vector3 position, out Quaternion facing) ||
            (preferredDistance > minimumDistance &&
             TryFindPhaseTwoPosition(direction, minimumDistance, out position, out facing)))
        {
            _gameObjectPivot.SetPositionAndRotation(position, facing);
        }

        SetLastPosition(_gameObjectPivot.position);
    }

    private bool TryFindPhaseTwoPosition(Vector3 direction, float distance, out Vector3 position, out Quaternion facing)
    {
        GetBossCapsule(out Vector3 bottom, out Vector3 top, out float radius);
        Vector3 playerPosition = _playerPoint.position;
        Vector3 bossPosition = _gameObjectPivot.position;
        Quaternion inverseRotation = Quaternion.Inverse(_gameObjectPivot.rotation);
        int arenaWalls = LayerMask.GetMask("ArenaWall");
        Vector3 lineHeight = Vector3.up * radius;

        for (int i = 0; i < 8; i++)
        {
            Vector3 offset = Quaternion.AngleAxis(i * 45f, Vector3.up) * direction * distance;
            position = playerPosition + offset;
            position.y = bossPosition.y;
            facing = Quaternion.LookRotation(-offset);
            if (Physics.Linecast(playerPosition + lineHeight, position + lineHeight, arenaWalls,
                    QueryTriggerInteraction.Ignore))
                continue;

            Quaternion turn = facing * inverseRotation;
            Vector3 capsuleOrigin = position + Vector3.up * CollisionSkin;
            Collider[] overlaps = Physics.OverlapCapsule(capsuleOrigin + turn * (bottom - bossPosition),
                capsuleOrigin + turn * (top - bossPosition), radius, Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);
            if (!System.Array.Exists(overlaps, collider => !collider.transform.IsChildOf(transform)))
                return true;
        }

        position = bossPosition;
        facing = _gameObjectPivot.rotation;
        return false;
    }

    private void GetBossCapsule(out Vector3 bottom, out Vector3 top, out float radius)
    {
        Transform capsuleTransform = _bossCapsule.transform;
        float fullRadius = GetCapsuleRadius(_bossCapsule);
        float halfSegment = Mathf.Max(0f,
            _bossCapsule.height * Mathf.Abs(capsuleTransform.lossyScale.y) * 0.5f - fullRadius);
        Vector3 center = capsuleTransform.TransformPoint(_bossCapsule.center);
        bottom = center - capsuleTransform.up * halfSegment;
        top = center + capsuleTransform.up * halfSegment;
        radius = Mathf.Max(fullRadius - CollisionSkin, fullRadius * 0.5f);
    }

    private static float GetCapsuleRadius(CapsuleCollider capsule)
    {
        Vector3 scale = capsule.transform.lossyScale;
        return capsule.radius * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));
    }

    private static float GetHorizontalReach(CapsuleCollider capsule, Vector3 origin)
    {
        Vector3 offset = capsule.transform.TransformPoint(capsule.center) - origin;
        offset.y = 0f;
        return offset.magnitude + GetCapsuleRadius(capsule);
    }

    public void AttackCloseRange(Vector3 gameObjectPosition, Vector2 gameObjectRotation)
    {
        Model.AttackCloseRange(gameObjectPosition, gameObjectRotation);
    }

    public void AttackLongRange(Vector3 gameObjectPosition, Vector2 gameObjectRotation)
    {
        Model.AttackLongRange(gameObjectPosition, gameObjectRotation);
    }
}
