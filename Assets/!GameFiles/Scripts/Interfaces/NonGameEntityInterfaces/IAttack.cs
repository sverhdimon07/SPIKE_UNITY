public interface IAttack
{
    public void Initialize(IDamageCalculator damageCalculator); //хз, но почему бы и нет

    public void Attack(Weapon weapon, Character damageReciever);
}
