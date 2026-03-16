public abstract class WeaponLongRange : Weapon
{
    /*
    [Range(10f, 100f)]
    [SerializeField] private float _range;
    */

    protected float _range;

    public float Range => _range;

    public abstract override void Initialize(float damage, float attackRange);

    //public abstract void InitializeRange(float range);
}
