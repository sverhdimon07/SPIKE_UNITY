using UnityEngine;

public abstract class PlayerAttack
{
    private IEnvironmentAreaAnalyzer _environmentAreaAnalyzer;

    private IDamageCalculator _damageCalculator;

    public void Initialize(IEnvironmentAreaAnalyzer environmentAreaAnalyzer, Vector3 playerPosition, Vector2 playerDirection, Vector3 weaponPrefabScale, float weaponAttackRange, IDamageCalculator damageCalculator)
    {
        _environmentAreaAnalyzer = environmentAreaAnalyzer;
        _environmentAreaAnalyzer.Initialize(playerPosition, playerDirection, weaponPrefabScale, weaponAttackRange);

        _damageCalculator = damageCalculator;
        //_damageCalculator.Initialize(); хз, всегда ли надо инитить, если внутри класса одна более-менее проста€ функци€
    }

    public void Attack(Weapon weapon, Vector3 playerPosition, Vector2 playerDirection)
    {
        IDamageable damageReciever = _environmentAreaAnalyzer.Analyze(playerPosition, playerDirection); //возможно стоит все подобные Character'у игровые сущности наследовать от единого Entity, но у нас же есть IDamageable (√≈Ќ»јЋ№Ќќ)

        if (damageReciever == null)
        {
            return;
        }
        float damage = _damageCalculator.Calculate(weapon);
        
        damageReciever.ApplyDamage(damage);
    }
}
