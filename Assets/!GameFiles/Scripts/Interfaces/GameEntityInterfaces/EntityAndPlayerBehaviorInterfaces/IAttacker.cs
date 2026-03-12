public interface IAttacker
{
    public void Attack(IAttack attack, IDamageCalculator damageCalculator, Weapon weapon, Character damageReciever); //хз, нужны ли входные параметры
}
