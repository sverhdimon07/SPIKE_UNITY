using UnityEngine; //нужен ли MonoBehaviour?

public class PlayerAttackLongRange : IAttack
{
    private IDamageCalculator _damageCalculator;

    private float _range;

    public void Initialize(IDamageCalculator damageCalculator)
    {
        Inject(damageCalculator);
    }

    public void Attack(Weapon weapon, Character damageReciever)
    {
        float damage = _damageCalculator.CalculateDamage(weapon);

        damageReciever.ApplyDamage(damage);
    }

    private void Inject(IDamageCalculator damageCalculator) //хз объединять ли такие методы в 1 Inject
    {
        _damageCalculator = damageCalculator;
    }
}
