public class PlayerWeaponCloseRange
{
    private WeaponCloseRange _weapon;

    public WeaponCloseRange Weapon => _weapon;

    public void Initialize(WeaponCloseRange weapon)
    {
        _weapon = weapon;
    }
}
