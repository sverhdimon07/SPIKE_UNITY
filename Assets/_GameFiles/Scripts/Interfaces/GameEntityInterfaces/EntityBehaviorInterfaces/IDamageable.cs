public interface IDamageable //добавил свойства на текущее здоровье И на лимит поинтов здоровья - надо подумать про подобные дополнения контрактов во всех интерфейсах
{
    public float Health { get; set; }

    public float MaxHealth { get; set; }

    public void TakeDamage(float damage);

    public void Die();
}
