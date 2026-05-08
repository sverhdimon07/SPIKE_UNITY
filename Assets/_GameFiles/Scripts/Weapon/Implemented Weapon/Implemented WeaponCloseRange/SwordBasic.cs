public sealed class SwordBasic : WeaponCloseRange //вот тут уже sealed излишен, так как скорее всего это будет тот самый обычный класс, от которого будут наследоваться мега-специфичные детализационные типы; ХОТЯ я тут так подумал, так а почему все равно не сделать контракт абстрактным классом
{
    public override void Initialize(WeaponData data)
    {
        _damage = data.Damage;
        _range = data.Range;
    }
}
