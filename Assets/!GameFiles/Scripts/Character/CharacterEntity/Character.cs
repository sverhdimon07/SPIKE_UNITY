using UnityEngine;

public abstract class Character : MonoBehaviour, IAttacker, IDamageable //я бы еще накидал контрактов на неуправляемое перемещение
{
    public void Attack(IAttack attack, IDamageCalculator damageCalculator, Weapon weapon, Character damageReciever)
    {
        //_controller.Attack(attack);
    }

    public void ApplyDamage(float damage)
    {
        //_playerController.PlayerHealthController....
    }
}
