public sealed class CharacterMechanicStateMachine
{
    private CharacterMechanicState _state; //по идее, я могу прописать вызов метода Enter в сеттере свойства, но пока хз, нормальная ли это практика

    public CharacterMechanicState State => _state;

    public CharacterMechanicStateMachine(Character character, CharacterMechanicState startState)
    {
        _state = startState;

        _state.Enter(character);
    }

    public void SwitchState(Character character, CharacterMechanicState nextState) //интересный момент с тем, что вроде бы делать подобные методы - соблюдение OSP, но и при этом любой сможет закинуть сюда что он захочет (ВИДИК САКУТИНА)
    {
        if (nextState == _state)
        {
            return;
        }
        _state.Exit(character);

        _state = nextState;

        _state.Enter(character); //интересный момент с тем, у кого этот метод вызывать - у обновившегося поля ИЛИ у локальной переменной (посмотреть ссылочные типы и типы значения)
    }//ИНТЕРЕСНОЕ ЗАМЕЧАНИЕ ПО ПОВОДУ ИНКАПСУЛЯЦИИ ЗДЕСЬ - раньше я никак не проверял входящий стейт на правильность (типо, не засунули ли нам сюда стейт, который уже активен)
}
