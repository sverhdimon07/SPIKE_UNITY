using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public abstract class Character : MonoBehaviour, IAttacker, IDamageable //я бы еще накидал контрактов на неуправляемое перемещение
{
    public void Attack()
    {
        //_controller.Attack(attack);
    }

    public void TakeDamage(float damage)
    {
        print("НАНЕСЕН УРОН ПРОТИВНИКУ");
        //_playerController.PlayerHealthController...
    }
}
