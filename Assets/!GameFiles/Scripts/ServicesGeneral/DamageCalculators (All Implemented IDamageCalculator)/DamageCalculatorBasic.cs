using UnityEngine; //нужен ли MonoBehaviour?

public class DamageCalculatorBasic : IDamageCalculator
{
    public void Initialize()
    {

    }

    public float CalculateDamage(Weapon weapon) //хз по семантике названия тут
    {
        float damage = weapon.Damage;

        return damage;
    }
}
