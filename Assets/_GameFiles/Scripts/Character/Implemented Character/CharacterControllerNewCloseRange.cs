using UnityEngine;

public enum AttackType
{
    Ice,
    Lighning,
    Lighning2,
    Last
}

public enum WeaponType
{
    First,
    Second
}

public class CharacterControllerNewCloseRange : CharacterControllerNew, ICloseRangeAttacker
{
    public WeaponType WeaponType;
    public AttackType AttackType;

    [Header("Block Settings")]
    [SerializeField] private float blockDuration = 2f;
    [SerializeField] private float blockCooldown = 8f;
    [SerializeField] private float blockChance = 1f;

    private bool _isBlocking;
    private float _blockTimer;
    private float _timeSinceLastBlock;
    private float _lastAttackTime;

    // ADDED: флаг для отслеживания входа в ближнюю зону
    private bool _wasInMeleeRange = false;

    public override void Awake()
    {
        base.Awake();

        if (WeaponType == WeaponType.First)
        {
            _firstSword.SetActive(true);
            _secondSword.SetActive(false);
        }
        else if (WeaponType == WeaponType.Second)
        {
            _firstSword.SetActive(false);
            _secondSword.SetActive(true);
        }
    }

    public override void Update()
    {
        base.Update();

        _gameObjectPivot.LookAt(_playerPoint);

        float distance = Vector3.Distance(transform.position, _playerPoint.position);

        if (distance > 1.1f)
        {
            // Далеко от игрока – двигаемся к нему, сбрасываем блок и флаг входа
            if (_isBlocking)
            {
                Unblock();
                _isBlocking = false;
                _blockTimer = 0f;
            }
            _wasInMeleeRange = false; // ADDED: сброс флага при выходе
            Locomote(new Vector2(transform.forward.x, transform.forward.z));
            return;
        }

        // Близко – проверяем, только ли вошли
        // ADDED: принудительный блок при первом входе
        if (!_wasInMeleeRange)
        {
            _isBlocking = true;
            _blockTimer = 4f;          // блок на 4 секунды
            Block();
            _timeSinceLastBlock = Time.time;
            _wasInMeleeRange = true;
        }

        // Обновляем блок (он обработает таймер)
        UpdateBlock();

        if (_isBlocking)
        {
            // В блоке не атакуем
            return;
        }

        // Атакуем только если прошло время после предыдущей атаки
        if (Time.time - _lastAttackTime >= 2f)
        {
            AttackCloseRange(_gameObjectPivot.position, new Vector2(_gameObjectPivot.forward.x, _gameObjectPivot.forward.z));
            _lastAttackTime = Time.time;
        }
    }

    private void UpdateBlock()
    {
        if (_isBlocking)
        {
            _blockTimer -= Time.deltaTime;
            if (_blockTimer <= 0f)
            {
                Unblock();
                _isBlocking = false;
                _blockTimer = 0f;
                _timeSinceLastBlock = Time.time;
                //Debug.Log("Выход из блока");
            }
            return;
        }

        // Начинаем блок, если прошёл кулдаун и сработала вероятность
        if (Time.time - _timeSinceLastBlock >= blockCooldown &&
            Random.value < blockChance)
        {
            _isBlocking = true;
            _blockTimer = blockDuration;
            _timeSinceLastBlock = Time.time;
            Block();
            //Debug.Log("Вход в блок");
        }
    }

    public void AttackCloseRange(Vector3 gameObjectPosition, Vector2 gameObjectRotation)
    {
        Model.AttackCloseRange(gameObjectPosition, gameObjectRotation);
    }
}
