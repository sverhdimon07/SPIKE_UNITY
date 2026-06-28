using System;
using UnityEngine;
using UnityEngine.Events;

public /*тут надо поработать с абстракцией*/ sealed class CharacterHealth //Эта реализация буквально дублируется в Character, то есть, это сервис, который можно РЕЮЗАТЬ И ПОДМЕНЯТЬ (ПЕРЕДЕЛАТЬ);
{
    public UnityAction Died;
    //public static UnityAction DiedSomeone;

    //public UnityAction Died;

    public UnityAction<float> DamageTaken;

    private readonly float _maxHealthValue;

    private float _healthValue; //потом мб переведем поля здоровья и демеджа на int везде (надо понять, насколько это оправдано и что стоит ставить); подумать про семантику названия этого поля (можно оставить, а можно назвать это поле value)

    public CharacterHealth(float maxHealth, float health) //ИНКАПСУЛЯЦИЯ (НАДО ПОТОМ СДЕЛАТЬ ВЕЗДЕ) - можно сделать простую проверку прям здесь ИЛИ можно изменить подход к иниту полей и инитить не сами поля, а свойства с условием в сеттере;надо ли делать эту проверку в классах более высокого уровня?
    {
        if (maxHealth <= 0f) //изначально написал тут так - if ((_maxHealth <= 0f) && (_maxHealth > 100f))... но это же чушь полная, ты задаешь в инспекторе максимальное значение, так почему оно вообще чем-то ограничивается? Это ошибка логики. ВСЕГДА ДУМАЙ О РАСШИРЕНИИ СИСТЕМЫ И ПЕРЕВОДЕ КАКОЙ-ТО КОНКРЕТНОЙ РЕАЛИЗАЦИИ В АБСТРАКТНЫЙ СЕРВИС, это помогает избежать подобных замыливаний глаз
        {
            throw new ArgumentOutOfRangeException();
        }
        _maxHealthValue = maxHealth;

        if ((health <= 0f) && (health > maxHealth)) //
        {
            throw new ArgumentOutOfRangeException();
        }
        _healthValue = health;
    }
    
    public float MaxHealthValue => _maxHealthValue;
    public float HealthValue => _healthValue;

    public void SetHealthValue(float healthValue)
    {
        if (healthValue <= 0f) //< 0, так как задел под расширение
        {
            throw new ArgumentOutOfRangeException();
        }
        if (healthValue > _maxHealthValue)
        {
            throw new ArgumentOutOfRangeException();
        }
        _healthValue = healthValue;
    }

    public void TakeDamage(float damage)
    {
        if (damage < 0f) //< 0, так как задел под расширение
        {
            throw new ArgumentOutOfRangeException();
        }
        if ((_healthValue -= damage) < 0f)
        {
            _healthValue = 0f;

            Die();
        }
        else if ((_healthValue -= damage) == 0f)
        {
            Die();
        }
        else
        {
            _healthValue -= damage;

            DamageTaken.Invoke(_healthValue);
        }
    }

    public void Heal()
    {
        _healthValue = _maxHealthValue;
    }

    public void Die()
    {
        Died.Invoke();
        //Died.Invoke();
        //DiedSomeone.Invoke();

        _healthValue = _maxHealthValue;
    }
}
