using UnityEngine.Events;

public sealed class PlayerHealthController
{
    private readonly PlayerHealth _health = new PlayerHealth();
    
    ~ PlayerHealthController()
    {
        _health.DamageTaken -= delegate () { DamageTaken.Invoke(); };
        _health.Died -= delegate () { Died.Invoke(); };
    }

    public UnityAction DamageTaken;
    public UnityAction Died;

    public void Initialize(float health)
    {
        _health.Initialize(health);

        _health.DamageTaken += delegate () { DamageTaken.Invoke(); };
        _health.Died += delegate () { Died.Invoke(); };
    }

    public void TakeDamage(float damage)
    {
        _health.TakeDamage(damage);
    }

    public void Die()
    {
        _health.Die();
    }

    public float GetHealth()
    {
        return _health.Health;
    }
}
