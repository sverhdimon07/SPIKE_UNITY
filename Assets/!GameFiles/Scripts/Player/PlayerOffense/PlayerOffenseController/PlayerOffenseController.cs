using UnityEngine;

public class PlayerOffenceController
{
    private readonly PlayerWeaponController _weaponController = new PlayerWeaponController();
    
    private readonly PlayerAttackCloseRange _attackCloseRange = new PlayerAttackCloseRange();

    private readonly PlayerAttackLongRange _attackLongRange = new PlayerAttackLongRange();

    public void Initialize(WeaponCloseRange weaponCloseRange, WeaponLongRange weaponLongRange, Vector3 playerPosition, Vector2 playerDirection, Vector3 weaponPrefabScale, float weaponAttackRange)
    {
        _weaponController.Initialize(weaponCloseRange, weaponLongRange);
        _attackCloseRange.Initialize(new EnvironmentAreaOverlapAnalyzer(), playerPosition, playerDirection, weaponPrefabScale, weaponAttackRange, new DamageCalculatorBasic()); //работа с конретной реализацией должна проводитьс€ наверху, но с другой стороны мы создаем крепкий контрактна то, что в €чейку ближней атаки не заинититс€ дальн€€ атака (что по идее может прилететь с сервера спокойно)
        _attackLongRange.Initialize(new EnvironmentAreaOverlapAnalyzer(), playerPosition, playerDirection, weaponPrefabScale, weaponAttackRange, new DamageCalculatorBasic()); //работа с конретной реализацией должна проводитьс€ наверху, но с другой стороны мы создаем крепкий контрактна то, что в €чейку ближней атаки не заинититс€ дальн€€ атака (что по идее может прилететь с сервера спокойно)
    }

    public void AttackCloseRange(Vector3 playerPosition, Vector2 playerDirection) //сначала прописал Inject тут, но зачем при каждой атаке че-то инджектить. »нджектить нужно при подн€тии нового оружи€, но так как у нас данной механики пока нет, мы инджектим при создании и инициализации оружи€ в руках игрока
    {
        _attackCloseRange.Attack(_weaponController.WeaponCloseRange, playerPosition, playerDirection);
    }

    public void AttackLongRange(Vector3 playerPosition, Vector2 playerDirection)
    {
        _attackLongRange.Attack(_weaponController.WeaponLongRange, playerPosition, playerDirection);
    }
}
