public sealed class PlayerWeaponCloseRange
{
    private Weapon _weapon;

    public Weapon Weapon => _weapon;

    public void Initialize(Weapon weapon)
    {
        _weapon = weapon;
    }
}
