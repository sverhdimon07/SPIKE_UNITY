using UnityEngine;

public class GameControllerTestScene : MonoBehaviour
{
    private InputController _inputController;

    private SwordBasic _swordBasic;

    private RocketLauncherBasic _rocketLauncherBasic;

    private Player _player;

    private void OnEnable()
    {
        InitializeInputController();
        InitializeSwordBasic(2f, 0f); //МГ, но легитимное (тут можно работать с МГ и конкретными реализациями, ибо здесь я создаю ИЛИ нахожу на сцене И иничу все дочерние системы)
        InitializeRocketLauncherBasic(1f, 10f); //МГ, но легитимное (тут можно работать с МГ и конкретными реализациями, ибо здесь я создаю ИЛИ нахожу на сцене И иничу все дочерние системы)
        InitializePlayer(); //хз, как под другому, но даже если они запускаются в Awake - OnEnable запускается раньше. Мб если сериализировать эти поля, то все будет норм

        _inputController._attackCloseRangeButtonPressed += _player.AttackCloseRange;
        _inputController._attackLongRangeButtonPressed += _player.AttackLongRange;
        _inputController._locomotionDirectionDirected += _player.LocomoteInUpdate;
        _inputController._runningButtonHolded += _player.RunInUpdate;
    }

    private void OnDisable()
    {
        _inputController._attackCloseRangeButtonPressed -= _player.AttackCloseRange;
        _inputController._attackLongRangeButtonPressed -= _player.AttackLongRange;
        _inputController._locomotionDirectionDirected -= _player.LocomoteInUpdate;
        _inputController._runningButtonHolded -= _player.RunInUpdate;
    }

    public void Initialize() //именно в этом классе не так важно использовать такой метод, чтобы в него потом вкладывать дочерние методы. Я бы вообще так не делал, ибо не разберусь потом в большом потоке входных данных, лучше упразднить этот метод и вызывать дочерние
    {
        //
    }

    private void InitializePlayer() //пока что я делаю поля с игроком и оружием, возможно это излишне и можно создавать сущности в локальных переменных, НО Я НЕ ДУМАЮ ТАК (но при этом дочерний монобех PlayerInputController у Player создается в локальной переменной, так как он нам тут не нужен, этот класс не имеет такой ответственности)
    {
        _player = FindAnyObjectByType<Player>(); //пока что я ничего на сцене не создаю кодом, ибо не придумал архитектуру спавнеров

        //Не понимаю, почему они удалили AddComponent из UnityEngine библиотеки

        Animator animator = _player.GetComponent<Animator>();

        _player.Initialize(animator, 2.5f, _swordBasic, _rocketLauncherBasic, _player.transform.position, _inputController.PlayerDirection, FindAnyObjectByType<SwordBasic>().transform.localScale, FindAnyObjectByType<RocketLauncherBasic>().Range); //пока в DI даем зависимость так (про speed = 2.5f вручную, new PlayerAttackCloseRange()) - типо имитируем прилет инфы с сервера/GameController'a
        //в ините игрока лютая дичь в конечных параметрах - да вообще все эти параметры надо как-то через сериализованные поля прогидывать (это настраивание среды для геймдизайнера)
    }

    private void InitializeSwordBasic(float damage, float range)
    {
        _swordBasic = FindAnyObjectByType<SwordBasic>(); //по идее все верно (НО ХЗ) - мы создаем оружие здесь именно в контексте нашей концепции, ибо мы его можем создавать и не в руке игрока, А ТАКЖЕ это не личное оружие игрока, это оружие которое может подобрать любой Character

        _swordBasic.Initialize(damage, range);
    }

    private void InitializeRocketLauncherBasic(float damage, float range)
    {
        _rocketLauncherBasic = FindAnyObjectByType<RocketLauncherBasic>(); //по идее все верно (НО ХЗ) - мы создаем оружие здесь именно в контексте нашей концепции, ибо мы его можем создавать и не в руке игрока, А ТАКЖЕ это не личное оружие игрока, это оружие которое может подобрать любой Character

        _rocketLauncherBasic.Initialize(damage, range);
    }

    public void InitializeInputController()
    {
        _inputController = FindAnyObjectByType<InputController>();

        _inputController.Initialize();
    }
}
