public class CharacterHealthController
{
    private readonly CharacterHealth _health = new CharacterHealth();

    public void Initialize(float health)
    {
        _health.Initialize(health);
    }

    public void TakeDamage(float damage)
    {
        _health.TakeDamage(damage);
    }

    public float GetHealth()
    {
        return _health.Health;
    }
}
