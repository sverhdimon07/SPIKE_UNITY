using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerHealth 
{
    private float _health;//потом мб переведем пол€ здоровь€ и демеджа на int везде (надо пон€ть, насколько это оправдано и что стоит ставить); подумать про семантику названи€ этого пол€ (можно оставить, а можно назвать это поле value)

    public UnityAction DamageTaken;
    public UnityAction Died;

    public float Health => _health;

    public void Initialize(float health) //»Ќ јѕ—”Ћя÷»я (Ќјƒќ ѕќ“ќћ —ƒ≈Ћј“№ ¬≈«ƒ≈) - можно сделать простую проверку пр€м здесь »Ћ» можно изменить подход к иниту полей и инитить не сами пол€, а свойства с условием в сеттере;надо ли делать эту проверку в классах более высокого уровн€?
    {
        if (health <= 0)
        {
            throw new ArgumentOutOfRangeException();
        }
        _health = health;
    }

    public void TakeDamage(float damage)
    {
        if (damage < 0) //< 0, так как задел под расширение
        {
            throw new ArgumentOutOfRangeException();
        }
        if ((_health -= damage) < 0)
        {
            _health = 0;

            Died?.Invoke();
            int activeSceneIndex = SceneManager.GetActiveScene().buildIndex; //понимаю, что дубл€ж и не следование SRP - потом переделаю
            SceneManager.LoadScene(activeSceneIndex);
        }
        else if ((_health -= damage) == 0)
        {
            Died?.Invoke();
            int activeSceneIndex = SceneManager.GetActiveScene().buildIndex; //понимаю, что дубл€ж и не следование SRP - потом переделаю
            SceneManager.LoadScene(activeSceneIndex);
        }
        else
        {
            _health -= damage;

            DamageTaken?.Invoke();
        }
    }
}
