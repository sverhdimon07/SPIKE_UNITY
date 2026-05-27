public sealed class CharacterHealthController
{
    private readonly CharacterHealth _health;

    public CharacterHealth Health => _health; //не совсем удачное название - поработать с именованием класса и этого поля
    
    public CharacterHealthController(CharacterHealth health)
    {
        _health = health;
    }
}
