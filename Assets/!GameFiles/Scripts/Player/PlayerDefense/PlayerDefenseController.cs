using UnityEngine;
using UnityEngine.InputSystem; // Новый Input System package
using UnityEngine.Events;

public sealed class PlayerDefenseController
{
    // Readonly поле сервиса - строгое следование паттерну из PlayerHealthController
    private readonly PlayerDefense _defense = new PlayerDefense();

    private readonly InputAction _defendAction;

    // Конструктор с внедрением зависимости (как и договаривались)
    // Делаем приватные сеттеры для полей, если понадобится мокинг в тестах
    public PlayerDefenseController(InputAction defendAction)
    {
        _defendAction = defendAction ?? throw new System.ArgumentNullException(nameof(defendAction));
    }

    // События наружу (публичный API для UI, Audio, VFX)
    public UnityAction OnDefenseStarted;
    public UnityAction OnDefenseEnded;

    public void Initialize()
    {
        // Подписка на события сервиса и проброс их наружу (паттерн из PlayerHealthController)
        _defense.DefenseStarted += () => OnDefenseStarted?.Invoke();
        _defense.DefenseEnded += () => OnDefenseEnded?.Invoke();

        // Подписка на ввод
        _defendAction.performed += OnDefendInput;
        _defendAction.canceled += OnDefendInput;

        _defendAction.Enable();
    }

    // Обработчик ввода: универсальный для нажатия и отпускания
    private void OnDefendInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _defense.Activate();
        }
        else if (context.canceled)
        {
            _defense.Deactivate();
        }
    }

    // === PUBLIC API ДЛЯ ВНЕШНИХ СИСТЕМ ===

    // Основной метод для CombatManager: "Стоит ли блокировать этот урон?"
    public bool IsDefending => _defense.IsDefending;

    // Алиас для семантической ясности в месте вызова (опционально)
    public bool ShouldBlockIncomingDamage() => _defense.ShouldBlockDamage();

    // Метод для экстренной отмены (например, при стане или оглушении)
    public void ForceDeactivate() => _defense.Deactivate();
}


/*
// как использовать в коде
// Пример: EnemyAttackController.cs
public void DealDamage(PlayerHealthController targetHealth, PlayerDefenseController targetDefense, float damage)
{
    // ВАРИАНТ А: Проверка защиты ПЕРЕД нанесением урона
    if (targetDefense != null && targetDefense.IsDefending)
    {
        // Опционально: проиграть звук "отскока" или частицы блока
        // OnBlockEffect?.Invoke();
        return; // Урон не наносится, метод прерывается
    }

    // Если защиты нет - наносим урон как обычно
    targetHealth.TakeDamage(damage);
}
*/