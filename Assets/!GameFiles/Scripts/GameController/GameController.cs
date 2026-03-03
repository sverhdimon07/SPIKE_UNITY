using UnityEngine;

public class GameController : MonoBehaviour
{
    private Player _player;

    private Weapon _weapon;

    private InputController _inputController;

    private void OnEnable()
    {
        InitializePlayer(); //хз, как под другому, но даже если они запускаются в Awake - OnEnable запускается раньше. Мб если сериализировать эти поля, то все будет норм
        InitializeWeapon(1f);
        InitializeInputController();

        _inputController._attackCloseRangeButtonPressed += _player.Attack;
        _inputController._attackLongRangeButtonPressed += _player.Attack;
        _inputController._locomotionDirectionDirected += _player.Locomote;
        _inputController._runningButtonHolded += _player.Run;
    }

    private void OnDisable()
    {
        _inputController._attackCloseRangeButtonPressed -= _player.Attack;
        _inputController._attackLongRangeButtonPressed -= _player.Attack;
        _inputController._locomotionDirectionDirected -= _player.Locomote;
        _inputController._runningButtonHolded -= _player.Run;
    }

    public void Initialize() //по идее все парамеры должны быть именно тут, а не во внутренних методах (ИСПРАВИТЬ У PLAYER И ДОПИСАТЬ У WEAPON)
    {
        //
    }

    private void InitializePlayer() //пока что я делаю поля с игроком и оружием, возможно это излишне и можно создавать сущности в локальных переменных, НО Я НЕ ДУМАЮ ТАК (но при этом дочерний монобех PlayerInputController у Player создается в локальной переменной, так как он нам тут не нужен, этот класс не имеет такой ответственности)
    {
        _player = FindAnyObjectByType<Player>(); //пока что я ничего на сцене не создаю кодом, ибо не придумал архитектуру спавнеров

        //Не понимаю, почему они удалили AddComponent из UnityEngine библиотеки

        Animator animator = _player.GetComponent<Animator>();

        _player.Initialize(animator, 2.5f, _weapon); //пока в DI даем зависимость так (про speed = 2.5f вручную, new PlayerAttackCloseRange()) - типо имитируем прилет инфы с сервера/GameController'a
    }

    private void InitializeWeapon(float damage)
    {
        _weapon = FindAnyObjectByType<Weapon>(); //по идее все верно (НО ХЗ) - мы создаем оружие здесь именно в контексте нашей концепции, ибо мы его можем создавать и не в руке игрока, А ТАКЖЕ это не личное оружие игрока, это оружие которое может подобрать любой Character

        _weapon.Initialize(damage);
    }

    public void InitializeInputController()
    {
        _inputController = FindAnyObjectByType<InputController>();

        _inputController.Initialize();
    }
}
