using System;
using UnityEngine.Events;

public sealed class PlayerDefense // Сервис, аналог PlayerHealth - чистый C#, легко мокается
{
    private bool _isDefending;

    // События для внешней реакции (анимации, звук, частицы)
    public UnityAction DefenseStarted;
    public UnityAction DefenseEnded;

    public bool IsDefending => _isDefending;

    public void Activate()
    {
        if (_isDefending) return; // Guard clause - защита от лишних аллокаций и вызовов
        _isDefending = true;
        DefenseStarted?.Invoke();
    }

    public void Deactivate()
    {
        if (!_isDefending) return;
        _isDefending = false;
        DefenseEnded?.Invoke();
    }

    // Метод-предикат для внешних систем (Вариант А: проверка ДО нанесения урона)
    // Возвращает true, если урон должен быть заблокирован
    public bool ShouldBlockDamage() => _isDefending;
}