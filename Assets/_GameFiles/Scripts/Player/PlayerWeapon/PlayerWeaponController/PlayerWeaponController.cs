public sealed class PlayerWeaponController
{
    private PlayerWeaponCloseRange _weaponCloseRange; //тут тоже надо зависеть от абстракции, как и в PlayerOffenseController

    private PlayerWeaponLongRange _weaponLongRange;

    //private IWeapon currentWeapon; - расширение под возможное переключение между ближним и дальним оружием при помощи прокрутки колеса мыши (но ТЗ пока требует другого); Здесь заметил ВАЖНЕЙШЕЕ ЗАМЕЧАНИЕ - в зависимости от того, как у игрока определено переключение между типами оружия (то есть, это может быть выбор через колесико ИЛИ же просто нажатие разных кнопок мыши (как у нас сейчас, что не особо используется в прям современных играх)), будет зависеть архитекура нашего кода (а точнее выбор раелизации, но это пока слишком комплексно и сложно сейчас делать) - то есть, выбор атаки может детерминироваться выбранным оружием (то есть, в текущий момент игрок может орудовать только одним оружием) ИЛИ же выбор атаки может детерминироваться нажатием необходимой кнопки (то есть, есть два слота для оружия и игрок может в моменте атаковать поочередно и тем, и другим без необходимости что-то выбирать)

    public Weapon WeaponCloseRange => _weaponCloseRange.Weapon;
    public Weapon WeaponLongRange => _weaponLongRange.Weapon;

    public void Initialize(PlayerWeaponCloseRange playerWeaponCloseRange, PlayerWeaponLongRange playerWeaponLongRange, Weapon weaponCloseRange, Weapon weaponLongRange)
    {
        _weaponCloseRange = playerWeaponCloseRange;
        _weaponLongRange = playerWeaponLongRange;
        _weaponCloseRange.Initialize(weaponCloseRange);
        _weaponLongRange.Initialize(weaponLongRange);
    }
}
