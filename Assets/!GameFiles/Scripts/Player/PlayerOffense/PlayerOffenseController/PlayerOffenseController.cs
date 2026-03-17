using UnityEngine;

public class PlayerOffenceController
{
    private readonly PlayerWeaponController _weaponController = new PlayerWeaponController();
    
    private readonly PlayerAttackCloseRange _attackCloseRange = new PlayerAttackCloseRange();

    private readonly PlayerAttackLongRange _attackLongRange = new PlayerAttackLongRange();

    public void Initialize(Vector3 position, Vector2 direction, WeaponCloseRange weaponCloseRange, WeaponLongRange weaponLongRange)
    {
        _weaponController.Initialize(weaponCloseRange, weaponLongRange);
        _attackCloseRange.Initialize(new EnvironmentAreaOverlapAnalyzerDamageable(), position, direction, weaponCloseRange.Range, new DamageCalculatorBasic()); //работа с конретной реализацией должна проводитьс€ наверху, но с другой стороны мы создаем крепкий контрактна то, что в €чейку ближней атаки не заинититс€ дальн€€ атака (что по идее может прилететь с сервера спокойно)
        _attackLongRange.Initialize(new EnvironmentAreaOverlapAnalyzerDamageable(), position, direction, weaponLongRange.Range, new DamageCalculatorBasic()); //работа с конретной реализацией должна проводитьс€ наверху, но с другой стороны мы создаем крепкий контрактна то, что в €чейку ближней атаки не заинититс€ дальн€€ атака (что по идее может прилететь с сервера спокойно)
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
