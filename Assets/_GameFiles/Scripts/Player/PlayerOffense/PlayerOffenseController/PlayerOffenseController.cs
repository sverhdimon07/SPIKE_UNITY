using UnityEngine;

public sealed class PlayerOffenseController
{
    private readonly PlayerWeaponController _weaponController;
    
    private readonly PlayerAttack _firstAttackType; //тут надо зависеть от абстракции, хот€ мы хотим чтобы были именно ближн€€ и дальн€€ атака, Ќќ лучше сделать зависимость от абстракции

    private readonly PlayerAttack _secondAttackType; //зависим от абстракции и в типе и а названии переменной (и тип, и название уже изменил)

    public PlayerAttack FirstAttackType => _firstAttackType;
    public PlayerAttack SecondAttackType => _secondAttackType;

    public PlayerOffenseController(PlayerWeaponController weaponController, PlayerAttack firstAttackType, PlayerAttack secondAttackType)
    {
        _weaponController = weaponController;
        _firstAttackType = firstAttackType;
        _secondAttackType = secondAttackType;
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
