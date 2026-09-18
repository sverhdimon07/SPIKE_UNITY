using UnityEngine;
using UnityEngine.UI;

public class CharacterControllerNewBoss : CharacterControllerNew, IAllRangesAttacker
{
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
    [SerializeField] private float phaseTwoTeleportDistance = 4f;
    [SerializeField] private float phaseOneAttackCooldown = 1.5f;
    [SerializeField] private float phaseTwoAttackCooldown = 0.8f;
    [SerializeField] private float phaseTwoStopDistance = 1.8f; // дистанция, на которой босс останавливается во 2-й фазе
    [SerializeField] private float initialSpawnDistance = 10f; // дистанция от игрока при первом появлении

    private int _currentGlobalLife;
    private bool _isPhaseTwoActive;

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

        if (_limiters != null)
            _limiters.SetActive(false);
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

        _gameObjectPivot.LookAt(_playerPoint);
        float distance = Vector3.Distance(transform.position, _playerPoint.position);

        if (_isPhaseTwoActive)
        {
            HandlePhaseTwoBehaviour(distance);
            return;
        }

        if (distance <= meleeRange)
            HandleMelee(distance);
        else
            HandleRanged(distance);
    }

    // --- ВТОРАЯ ФАЗА: агрессивный ближний бой с остановкой на дистанции ---
    private void HandlePhaseTwoBehaviour(float distance)
    {
        // 1. Двигаемся, только если дистанция больше заданной остановочной дистанции
        if (distance > phaseTwoStopDistance)
        {
            Locomote(new Vector2(_gameObjectPivot.forward.x, _gameObjectPivot.forward.z));
        }
        else
        {
            Idle(); // стоим на месте, когда подошли достаточно близко
        }

        // 2. Блок отключён
        if (_isBlocking)
        {
            Unblock();
            _isBlocking = false;
            _blockTimer = 0f;
        }

        // 3. Атакуем, если игрок в радиусе meleeRange
        if (distance <= meleeRange && Time.time - _lastAttackTime >= attackCooldown)
        {
            AttackCloseRange(_gameObjectPivot.position, new Vector2(_gameObjectPivot.forward.x, _gameObjectPivot.forward.z));
            _lastAttackTime = Time.time;
        }
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
        attackCooldown = phaseTwoAttackCooldown;

        TeleportBossToPlayer(phaseTwoTeleportDistance);

        if (_limiters != null)
        {
            _limiters.SetActive(true);
            _limiters.transform.SetParent(null);
            if (_limitersParticle != null)
                _limitersParticle.Play();
        }

        if (_isBlocking)
        {
            Unblock();
            _isBlocking = false;
            _blockTimer = 0f;
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

    private void TeleportBossToPlayer(float distance)
    {
        if (_playerPoint == null) return;

        Vector3 direction = (transform.position - _playerPoint.position);
        // Если босс и игрок почти в одной точке – выбираем случайное направление
        if (direction.sqrMagnitude < 0.01f)
        {
            direction = Random.insideUnitSphere;
            direction.y = 0;
            direction.Normalize();
        }
        else
        {
            direction.Normalize();
        }

        Vector3 targetPosition = _playerPoint.position + direction * distance;
        targetPosition.y = transform.position.y;
        transform.position = targetPosition;
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
