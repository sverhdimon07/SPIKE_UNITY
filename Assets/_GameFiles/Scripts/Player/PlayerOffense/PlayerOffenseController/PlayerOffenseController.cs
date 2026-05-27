using UnityEngine;

public sealed class PlayerOffenseController
{
    private PlayerWeaponController _weaponController = new PlayerWeaponController();
    
    private PlayerAttack _firstAttackType = new PlayerAttackCloseRange(); //тут надо зависеть от абстракции, хот€ мы хотим чтобы были именно ближн€€ и дальн€€ атака, Ќќ лучше сделать зависимость от абстракции

    private PlayerAttack _secondAttackType = new PlayerAttackLongRange(); //зависим от абстракции и в типе и а названии переменной (и тип, и название уже изменил)

    public PlayerOffenseController(Weapon weaponCloseRange, Weapon weaponLongRange, Vector3 gameObjectPosition, Vector2 renderAndSkeletonDirectionXZ)
    {
        //_weaponController = weaponController;
        _weaponController.Initialize(weaponCloseRange, weaponLongRange);
        _firstAttackType.Initialize(new EnvironmentAreaOverlapAnalyzer<IDamageable, PlayerController>(), gameObjectPosition, renderAndSkeletonDirectionXZ, 0.15f, weaponCloseRange.Range, 1.25f, new DamageCalculatorBasic()); //работа с конретной реализацией должна проводитьс€ наверху, но с другой стороны мы создаем крепкий контрактна то, что в €чейку ближней атаки не заинититс€ дальн€€ атака (что по идее может прилететь с сервера спокойно)
        _secondAttackType.Initialize(new EnvironmentAreaOverlapAnalyzer<IDamageable, PlayerController>(), gameObjectPosition, renderAndSkeletonDirectionXZ, 0.15f, weaponLongRange.Range, 1.25f, new DamageCalculatorBasic()); //работа с конретной реализацией должна проводитьс€ наверху, но с другой стороны мы создаем крепкий контрактна то, что в €чейку ближней атаки не заинититс€ дальн€€ атака (что по идее может прилететь с сервера спокойно)
    }

    public void AttackCloseRange(Vector3 gameObjectPosition, Vector2 direction) //сначала прописал Inject тут, но зачем при каждой атаке че-то инджектить. »нджектить нужно при подн€тии нового оружи€, но так как у нас данной механики пока нет, мы инджектим при создании и инициализации оружи€ в руках игрока
    {
        _firstAttackType.Attack(_weaponController.WeaponCloseRange, gameObjectPosition, direction);
    }

    public void AttackLongRange(Vector3 gameObjectPosition, Vector2 renderAndSkeletonDirectionXZ)
    {
        _secondAttackType.Attack(_weaponController.WeaponLongRange, gameObjectPosition, renderAndSkeletonDirectionXZ);
    }
}
