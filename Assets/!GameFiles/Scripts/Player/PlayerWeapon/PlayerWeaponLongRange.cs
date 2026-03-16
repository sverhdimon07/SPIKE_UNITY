public class PlayerWeaponLongRange
{
    private WeaponLongRange _weapon;

    public WeaponLongRange Weapon => _weapon;

    public void Initialize(WeaponLongRange weapon)
    {
        _weapon = weapon;
    }
}
