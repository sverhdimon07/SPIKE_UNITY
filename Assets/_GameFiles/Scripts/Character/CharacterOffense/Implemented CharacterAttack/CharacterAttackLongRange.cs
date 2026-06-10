using UnityEngine;
using UnityEngine.Events;

public class CharacterAttackLongRange : CharacterAttack
{
    public override void Attack(Weapon weapon, Vector3 position, Vector2 direction)
    {
        Attacked.Invoke();
        base.Attack(weapon, position, direction);
    }
}
