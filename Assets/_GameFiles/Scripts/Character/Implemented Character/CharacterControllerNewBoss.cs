using UnityEngine;
using UnityEngine.Events;

public class CharacterControllerNewBoss : CharacterControllerNew, IAllRangesAttacker
{
    public static int PlayerCompleteBossLives = 0;

    public UnityAction FirstStageStarted;
    public UnityAction SecondStageStarted;

    public static bool Spawned = false;
    public static bool FalseIfItIsFirstStage = true;

    [Header("Phase Settings (Global Lives)")]
    [SerializeField] private int globalLives = 2;          // общее количество фаз (жизней)
    [SerializeField] private GameObject _limiters;    // объект-ограничитель для ближней фазы
    [SerializeField] private ParticleSystem _limitersParticle;

    [Header("Phase Transition Teleports")]
    private Transform bossCenterPoint;    // точка, куда телепортируется босс после 2-й фазы
    [SerializeField] private float phaseTwoTeleportDistance = 5.5f;   // дистанция от игрока при переходе во 2-ю фазу
    [SerializeField] private float phaseTwoTeleportPlayerDistance = 10f; // дистанция от босса, куда отбрасывать игрока после 2-й фазы

    [Header("Attack Speeds")]
    [SerializeField] private float phaseOneAttackCooldown = 1.5f;  // скорость атак в 1-й фазе (ближние)
    [SerializeField] private float phaseTwoAttackCooldown = 0.8f;   // скорость атак во 2-й фазе (только ближние)

    [Header("All Ranges Settings (Phase 1)")]
    [SerializeField] private float meleeRange = 1.1f;          // дистанция ближнего боя
    [SerializeField] private float timeToRangedAttack = 5f;     // время на дистанции > meleeRange до первого дальнего выстрела
    [SerializeField] private float chaseDuration = 10f;         // длительность преследования после дальнего выстрела

    // Приватные переменные состояния
    private int _currentGlobalLife;
    private bool _isPhaseTwoActive;
    private float _currentAttackCooldown;   // текущий интервал атак (зависит от фазы)
    private float _lastAttackTime;           // время последней атаки (уже было, но продублируем)

    // Переменные для логики AllRanges (фаза 1)
    private float _timeSinceFar;
    private bool _hasDoneRangedAttack;
    private bool _isChasing;
    private float _chaseTimer;

    // Флаги для телепортации
    private bool _hasTeleportedToPhaseTwo;

    private void Start()
    {
        FirstStageStarted += SecondDeathToFirst;
        SecondStageStarted += FirstDeathToSecond;

        PlayerCompleteBossLives = 0;
        Spawned = false;
        FalseIfItIsFirstStage = false;

        bossCenterPoint = FindAnyObjectByType<Bootstrap>().transform;

        // Инициализация начальных значений
        _currentGlobalLife = 0;               // начинаем с 0-й фазы (первая)
        _isPhaseTwoActive = false;
        _currentAttackCooldown = phaseOneAttackCooldown;
        _lastAttackTime = -_currentAttackCooldown; // чтобы можно было атаковать сразу

        if (_limiters != null)
            _limiters.SetActive(false);
    }

    private void FirstDeathToSecond()
    {
        AnyDeath();
        _limiters.SetActive(true);
        _limiters.transform.SetParent(null);
        _limitersParticle.Play();
    }

    private void SecondDeathToFirst()
    {
        AnyDeath();
        _limiters.SetActive(false);
        _limiters.transform.SetParent(gameObject.transform);
        _limitersParticle.Stop();
    }

    private void Update()
    {
        print(FalseIfItIsFirstStage);

        if (FalseIfItIsFirstStage == false)
        {
            FirstStageStarted.Invoke();
        }
        else if (FalseIfItIsFirstStage == true)
        {
            SecondStageStarted.Invoke();
        }

        // Обновляем состояние FSM (если нужно)
        _model.MechanicStateMachine.State.DoWithinFrame(_model);

        float distance = Vector3.Distance(transform.position, _playerPoint.position);
        _gameObjectPivot.LookAt(_playerPoint);

        if (_isPhaseTwoActive)
        {
            // ФАЗА 2: только ближний бой, постоянное движение к игроку
            HandlePhaseTwoBehaviour(distance);
        }
        else
        {
            // ФАЗА 1: стандартное AllRanges поведение
            HandlePhaseOneBehaviour(distance);
        }
    }

    public void AnyDeath()  // переопределяем метод базового класса
    {
        // Босс "умер" (здоровье модели закончилось)
        _currentGlobalLife++;

        if (_currentGlobalLife < globalLives)
        {
            // Переход на следующую фазу
            if (!_isPhaseTwoActive && _currentGlobalLife == 1)
            {
                // Переход во вторую фазу
                ActivatePhaseTwo();
            }
            else
            {
                // Возврат в первую фазу после завершения второй (если globalLives > 2, но по заданию 2 фазы)
                ReturnToPhaseOne();
            }

            // Восстанавливаем здоровье модели (через HealthController)
            _model.HealthController.Health.Heal();  // предполагаем, что есть такой метод
            // Если в модели нет RestoreFullHealth, можно так:
            // _model.HealthController.Health.SetHealth(_model.HealthController.Health.MaxHealth);
        }
    }

    private void ActivatePhaseTwo()
    {
        _isPhaseTwoActive = true;
        _currentAttackCooldown = phaseTwoAttackCooldown;
        _hasTeleportedToPhaseTwo = false;   // ещё не телепортировались

        // Включаем ограничители арены
        if (_limiters != null)
            _limiters.SetActive(true);

        // Телепортируем босса к игроку (не впритык, на заданную дистанцию)
        TeleportBossToPlayer(phaseTwoTeleportDistance);

        // Сбрасываем таймеры AllRanges (на всякий случай)
        _timeSinceFar = 0f;
        _hasDoneRangedAttack = false;
        _isChasing = false;
        _chaseTimer = 0f;
    }

    private void ReturnToPhaseOne()
    {
        _isPhaseTwoActive = false;
        _currentAttackCooldown = phaseOneAttackCooldown;

        // Выключаем ограничители арены
        if (_limiters != null)
            _limiters.SetActive(false);

        // Телепортируем босса в центр
        if (bossCenterPoint != null)
            transform.position = bossCenterPoint.position;
        else
            Debug.LogWarning("BossCenterPoint не назначен!");

        // Телепортируем игрока подальше от босса
        TeleportPlayerAway(phaseTwoTeleportPlayerDistance);

        // Сбрасываем все таймеры AllRanges для новой первой фазы
        _timeSinceFar = 0f;
        _hasDoneRangedAttack = false;
        _isChasing = false;
        _chaseTimer = 0f;
        _lastAttackTime = -_currentAttackCooldown;
    }

    private void TeleportBossToPlayer(float distance)
    {
        if (_playerPoint == null) return;
        Vector3 directionToPlayer = (transform.position - _playerPoint.position).normalized;
        Vector3 targetPosition = _playerPoint.position + directionToPlayer * distance;
        // Дополнительно: можно ограничить по Y, чтобы босс не улетел
        targetPosition.y = transform.position.y;
        transform.position = targetPosition;
    }

    private void TeleportPlayerAway(float distance)
    {
        if (_playerPoint == null) return;
        Vector3 directionFromBoss = (_playerPoint.position - transform.position).normalized;
        Vector3 targetPosition = transform.position + directionFromBoss * distance;
        targetPosition.y = _playerPoint.position.y; // сохраняем высоту игрока
        _playerPoint.position = targetPosition;
    }

    private void HandlePhaseTwoBehaviour(float distance)
    {
        // Всегда двигаемся к игроку
        if (distance > meleeRange)
        {
            Locomote(new Vector2(_gameObjectPivot.forward.x, _gameObjectPivot.forward.z));
        }
        else
        {
            Idle(); // остановились в зоне атаки
        }

        // Атакуем с заданной скоростью, если в ближней зоне
        if (distance <= meleeRange && Time.time - _lastAttackTime >= _currentAttackCooldown)
        {
            AttackCloseRange(_gameObjectPivot.position, new Vector2(_gameObjectPivot.forward.x, _gameObjectPivot.forward.z));
            _lastAttackTime = Time.time;

            // Эффекты ближнего боя (Water)
            if (WaterEffects != null) WaterEffects.Play();
            if (WaterSound != null) WaterSound.Play();
        }
    }

    private void HandlePhaseOneBehaviour(float distance)
    {
        if (distance <= meleeRange)
        {
            // Ближняя зона: сбрасываем дальние таймеры, атакуем с phaseOneAttackCooldown
            _timeSinceFar = 0f;
            _hasDoneRangedAttack = false;
            _isChasing = false;
            _chaseTimer = 0f;

            Idle(); // стоим на месте

            if (Time.time - _lastAttackTime >= _currentAttackCooldown)
            {
                AttackCloseRange(_gameObjectPivot.position, new Vector2(_gameObjectPivot.forward.x, _gameObjectPivot.forward.z));
                _lastAttackTime = Time.time;

                if (WaterEffects != null) WaterEffects.Play();
                if (WaterSound != null) WaterSound.Play();
            }
        }
        else
        {
            // Дистанция > meleeRange – логика дальнего боя
            if (_isChasing)
            {
                _chaseTimer += Time.deltaTime;
                Locomote(new Vector2(_gameObjectPivot.forward.x, _gameObjectPivot.forward.z));

                if (distance <= meleeRange)
                {
                    _isChasing = false;
                    _chaseTimer = 0f;
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
    }

    private void PerformRangedAttack()
    {
        if (Time.time - _lastAttackTime >= _currentAttackCooldown)
        {
            AttackLongRange(_gameObjectPivot.position, new Vector2(_gameObjectPivot.forward.x, _gameObjectPivot.forward.z));


            _lastAttackTime = Time.time;

            if (FireEffects != null) FireEffects.Play();
            if (FireSound != null) FireSound.Play();
        }
    }

    public void AttackCloseRange(Vector3 gameObjectPosition, Vector2 gameObjectRotation)
    {
        _model.AttackCloseRange(gameObjectPosition, gameObjectRotation);
        WaterEffects.Play();
        WaterSound.Play();
    }

    public void AttackLongRange(Vector3 gameObjectPosition, Vector2 gameObjectRotation)
    {
        _model.AttackLongRange(gameObjectPosition, gameObjectRotation);
        LightningEffects.Play();
        LightningSound.Play();
    }
}
