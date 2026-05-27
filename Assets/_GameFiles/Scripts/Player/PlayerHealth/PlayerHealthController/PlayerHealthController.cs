public sealed class PlayerHealthController
{
    private readonly PlayerHealth _health;

    public PlayerHealth Health => _health; //не совсем удачное название - поработать с именованием класса и этого поля
    
    public PlayerHealthController(PlayerHealth health)
    {
        _health = health;
    }
}
