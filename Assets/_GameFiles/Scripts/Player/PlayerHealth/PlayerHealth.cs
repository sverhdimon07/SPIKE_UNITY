using System;
using UnityEngine.Events;

public sealed class PlayerHealth //Эта реализация буквально дублируется в Character, то есть, это сервис, который можно РЕЮЗАТЬ И ПОДМЕНЯТЬ (ПЕРЕДЕЛАТЬ);
{
    private readonly float _maxHealth;

    private float _health;//потом мб переведем поля здоровья и демеджа на int везде (надо понять, насколько это оправдано и что стоит ставить); подумать про семантику названия этого поля (можно оставить, а можно назвать это поле value)

    public UnityAction DamageTaken;
    public UnityAction Died;

    public PlayerHealth(float maxHealth, float health) //ИНКАПСУЛЯЦИЯ (НАДО ПОТОМ СДЕЛАТЬ ВЕЗДЕ) - можно сделать простую проверку прям здесь ИЛИ можно изменить подход к иниту полей и инитить не сами поля, а свойства с условием в сеттере;надо ли делать эту проверку в классах более высокого уровня?
    {
        if (_maxHealth <= 0f) //изначально написал тут так - if ((_maxHealth <= 0f) && (_maxHealth > 100f))... но это же чушь полная, ты задаешь в инспекторе максимальное значение, так почему оно вообще чем-то ограничивается? Это ошибка логики. ВСЕГДА ДУМАЙ О РАСШИРЕНИИ СИСТЕМЫ И ПЕРЕВОДЕ КАКОЙ-ТО КОНКРЕТНОЙ РЕАЛИЗАЦИИ В АБСТРАКТНЫЙ СЕРВИС, это помогает избежать подобных замыливаний глаз
        {
            throw new ArgumentOutOfRangeException();
        }
        _maxHealth = maxHealth;

        if ((health <= 0f) && (health > maxHealth)) //
        {
            throw new ArgumentOutOfRangeException();
        }
        _health = health;
    }

    public float MaxHealth => _maxHealth;
    public float Health => _health;

    public void TakeDamage(float damage)
    {
        if (damage < 0f) //< 0, так как задел под расширение
        {
            throw new ArgumentOutOfRangeException();
        }
        if ((_health -= damage) < 0f)
        {
            _health = 0f;

            Die();
        }
        else if ((_health -= damage) == 0f)
        {
            Die();
        }
        else
        {
            _health -= damage;

            DamageTaken.Invoke();
        }
    }

    public void Die()
    {
        Died.Invoke();
    }
}
