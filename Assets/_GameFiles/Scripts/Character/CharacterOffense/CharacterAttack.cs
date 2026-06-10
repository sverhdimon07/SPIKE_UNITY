using UnityEngine;
using UnityEngine.Events;

public abstract class CharacterAttack
{
    public UnityAction Attacked;

    private IEnvironmentAreaAnalyzer<IDamageable, CharacterControllerNew> _environmentAreaAnalyzer;

    private IDamageCalculator _damageCalculator;

    public void Initialize(IEnvironmentAreaAnalyzer<IDamageable, CharacterControllerNew> environmentAreaAnalyzer, Vector3 position, Vector2 direction, float environmentAreaAnalyzerToolDistanceToPlayer, float environmentAreaAnalyzerToolLength, float environmentAreaAnalyzerToolHeight, IDamageCalculator damageCalculator)
    {
        _environmentAreaAnalyzer = environmentAreaAnalyzer;
        _environmentAreaAnalyzer.Initialize(position, direction, environmentAreaAnalyzerToolDistanceToPlayer, environmentAreaAnalyzerToolLength, environmentAreaAnalyzerToolHeight);

        _damageCalculator = damageCalculator;
        //_damageCalculator.Initialize(); хз, всегда ли надо инитить, если внутри класса одна более-менее проста€ функци€
    }

    public virtual void Attack(Weapon weapon, Vector3 position, Vector2 direction)
    {
        IDamageable damageReciever = _environmentAreaAnalyzer.Analyze(position, direction); //возможно стоит все подобные Character'у игровые сущности наследовать от единого Entity, но у нас же есть IDamageable (√≈Ќ»јЋ№Ќќ)

        if (damageReciever == null)
        {
            return;
        }
        float damage = _damageCalculator.Calculate(weapon);
        
        damageReciever.TakeDamage(damage);
    }
}
