public class DamageCalculatorBasic : IDamageCalculator
{
    public void Initialize()
    {
        //нет доп модификаторов (они должны быть в других реализациях IDamageCalculator), поэтому тут пока пусто
    }

    public float Calculate(Weapon weapon)
    {
        float damage = weapon.Damage;

        return damage;
    }
}
