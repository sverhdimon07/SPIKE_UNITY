public sealed class PlayerMechanicStateMachine
{
    private PlayerMechanicState _state; //по идее, я могу прописать вызов метода Enter в сеттере свойства, но пока хз, нормальная ли это практика

    public PlayerMechanicState State => _state;

    public PlayerMechanicStateMachine(Player player, PlayerMechanicState startState)
    {
        _state = startState;

        _state.Enter(player, this);
    }

    public bool TrySwitchState(Player player, PlayerMechanicState nextState)
    {
        return _state.TryExit(player, this, nextState);
    }

    public void SwitchState(Player player, PlayerMechanicState nextState) //интересный момент с тем, что вроде бы делать подобные методы - соблюдение OSP, но и при этом любой сможет закинуть сюда что он захочет (ВИДИК САКУТИНА)
    {
        /*
        if (_state == nextState)
        {
            return;
        }*/
        /*
        if (nextState.GetType() == _state.GetType())
        {
            return;
        }*/
        //_state.Exit(player, this, nextState);

        _state = nextState;

        _state.Enter(player, this); //интересный момент с тем, у кого этот метод вызывать - у обновившегося поля ИЛИ у локальной переменной (посмотреть ссылочные типы и типы значения)
    }//ИНТЕРЕСНОЕ ЗАМЕЧАНИЕ ПО ПОВОДУ ИНКАПСУЛЯЦИИ ЗДЕСЬ - раньше я никак не проверял входящий стейт на правильность (типо, не засунули ли нам сюда стейт, который уже активен)
}
