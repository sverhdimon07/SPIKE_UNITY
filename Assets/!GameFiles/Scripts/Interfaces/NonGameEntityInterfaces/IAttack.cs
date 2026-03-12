public interface IAttack
{
    public void Initialize(IDamageCalculator damageCalculator); //”¡–¿“‹

    public void Attack(Weapon weapon, Character damageReciever);
}
