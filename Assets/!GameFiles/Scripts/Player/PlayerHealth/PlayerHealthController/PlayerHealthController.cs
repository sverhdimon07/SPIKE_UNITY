public class PlayerHealthController
{
    private readonly PlayerHealth _health = new PlayerHealth();

    public void Initialize(float health)
    {
        _health.Initialize(health);
    }

    public void TakeDamage(float damage)
    {
        _health.TakeDamage(damage);
    }
}
