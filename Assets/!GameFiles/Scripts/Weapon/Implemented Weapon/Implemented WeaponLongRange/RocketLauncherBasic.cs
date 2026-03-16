public class RocketLauncherBasic : WeaponLongRange
{
    public override void Initialize(float damage, float range)
    {
        _damage = damage;
        _range = range;

        //InitializeRange(Range);
    }
    /*
    public override void InitializeRange(float range)
    {
        _range = range;
    }*/
}
