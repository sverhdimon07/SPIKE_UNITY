public class PlayerMechanicStateMachine
{
    private Player _player;

    private PlayerMechanicState _state; //по идее, я могу прописать вызов метода Enter в сеттере свойства, но пока хз, нормальная ли это практика

    public PlayerMechanicState State => _state;

    public PlayerMechanicStateMachine(Player player)
    {
        _player = player;

        _state = new PlayerIdleMechanicState(); //

        _state.Enter(); //
    }

    public void SwitchState(PlayerMechanicState newState) //интересный момент с тем, что вроде бы делать подобные методы - соблюдение OSP, но и при этом любой сможет закинуть сюда что он захочет (ВИДИК САКУТИНА)
    {
        _state.Exit();

        _state = newState;

        _state.Enter(); //интересный момент с тем, у кого этот метод вызывать - у обновившегося поля ИЛИ у локальной переменной (посмотреть ссылочные типы и типы значения)
    }
}
