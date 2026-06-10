using UnityEngine;
using UnityEngine.Events;

public class CharacterAttackCloseRange : CharacterAttack
{
    public override void Attack(Weapon weapon, Vector3 position, Vector2 direction)
    {
        Attacked.Invoke();
        base.Attack(weapon, position, direction);
    }
}
