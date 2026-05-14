using UnityEngine;

public sealed class PlayerOffenseController
{
    private readonly PlayerWeaponController _weaponController = new PlayerWeaponController();
    
    private readonly PlayerAttackCloseRange _attackCloseRange = new PlayerAttackCloseRange();

    private readonly PlayerAttackLongRange _attackLongRange = new PlayerAttackLongRange();

    public PlayerOffenseController(Vector3 position, Vector2 direction, WeaponCloseRange weaponCloseRange, WeaponLongRange weaponLongRange)
    {
        _weaponController.Initialize(weaponCloseRange, weaponLongRange);
        _attackCloseRange.Initialize(new EnvironmentAreaOverlapAnalyzer<IDamageable, Player>(), position, direction, 0.15f, weaponCloseRange.Range, 1.25f, new DamageCalculatorBasic()); //работа с конретной реализацией должна проводитьс€ наверху, но с другой стороны мы создаем крепкий контрактна то, что в €чейку ближней атаки не заинититс€ дальн€€ атака (что по идее может прилететь с сервера спокойно)
        _attackLongRange.Initialize(new EnvironmentAreaOverlapAnalyzer<IDamageable, Player>(), position, direction, 0.15f, weaponLongRange.Range, 1.25f, new DamageCalculatorBasic()); //работа с конретной реализацией должна проводитьс€ наверху, но с другой стороны мы создаем крепкий контрактна то, что в €чейку ближней атаки не заинититс€ дальн€€ атака (что по идее может прилететь с сервера спокойно)
    }

    public void AttackCloseRange(Vector3 position, Vector2 direction) //сначала прописал Inject тут, но зачем при каждой атаке че-то инджектить. »нджектить нужно при подн€тии нового оружи€, но так как у нас данной механики пока нет, мы инджектим при создании и инициализации оружи€ в руках игрока
    {
        _attackCloseRange.Attack(_weaponController.WeaponCloseRange, position, direction);
    }

    public void AttackLongRange(Vector3 position, Vector2 direction)
    {
        _attackLongRange.Attack(_weaponController.WeaponLongRange, position, direction);
    }
}
