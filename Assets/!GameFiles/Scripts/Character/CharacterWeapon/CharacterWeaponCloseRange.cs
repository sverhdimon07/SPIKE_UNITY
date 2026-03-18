using UnityEngine;

public class CharacterWeaponCloseRange
{
    private WeaponCloseRange _weapon;

    public WeaponCloseRange Weapon => _weapon;

    public void Initialize(WeaponCloseRange weapon)
    {
        _weapon = weapon;
    }
}
