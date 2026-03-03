using UnityEngine; //нужен ли MonoBehaviour?

public class PlayerOffenceController
{
    private Weapon _weapon;

    private IAttack _attack;

    public void Initialize(Weapon weapon)
    {
        InitializeWeapon(weapon);
    }

    public void Attack(IAttack attack, IDamageCalculator damageCalculator, Weapon weapon, Character damageReciever)
    {
        Inject(attack, damageCalculator);

        _attack.Attack(weapon, damageReciever);
    }

    private void InitializeWeapon(Weapon weapon)
    {
        _weapon = weapon;
    }

    private void Inject(IAttack attack, IDamageCalculator damageCalculator) //странно
    {
        _attack = attack;

        InitializeAttack(damageCalculator);
    }

    private void InitializeAttack(IDamageCalculator damageCalculator)
    {
        _attack.Initialize(damageCalculator);
    }
}
