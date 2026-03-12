public interface IAttacker
{
    public void Attack(IAttack attack, IDamageCalculator damageCalculator, Weapon weapon, Character damageReciever);
    //public void Attack(Weapon weapon, IDamageCalculator damageCalculator, Character damageReciever);
}
