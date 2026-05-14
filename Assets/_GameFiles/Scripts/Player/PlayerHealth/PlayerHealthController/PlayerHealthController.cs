using UnityEngine.Events;

public sealed class PlayerHealthController
{
    private readonly PlayerHealth _health;
    
    public PlayerHealthController(PlayerHealth health)
    {
        _health = health;

        _health.DamageTaken += delegate () { DamageTaken.Invoke(); };
        _health.Died += delegate () { Died.Invoke(); };
    }

    ~ PlayerHealthController()
    {
        _health.DamageTaken -= delegate () { DamageTaken.Invoke(); };
        _health.Died -= delegate () { Died.Invoke(); };
    }

    public UnityAction DamageTaken;
    public UnityAction Died;

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
