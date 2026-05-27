using UnityEngine;
using UnityEngine.Events;

public class CharacterAttackLongRange : CharacterAttack
{
    public static UnityAction Attacked;

    public override void Attack(Weapon weapon, Vector3 position, Vector2 direction)
    {
        Attacked.Invoke();
        base.Attack(weapon, position, direction);
    }
}
