public class SwordBasic : WeaponCloseRange
{
    public override void Initialize(WeaponData data)
    {
        _damage = data.Damage;
        _range = data.Range;
    }
}
