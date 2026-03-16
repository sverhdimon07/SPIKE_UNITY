using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    /*
    [Range(1f, 100f)]
    [SerializeField] private float _damage;
    */

    protected float _damage; //пока хз, как не делать пол€ абстрактного класса protected, мб то что их нельз€ использовать из-за нарушени€ инкапсул€ции - это ложь, ибо € сам логически никогда этого тейка не понимал

    public float Damage => _damage;

    public abstract void Initialize(float damage, float attackRange); //пока хз, как работать с тем, когда у дочерних абстракций по€вл€ютс€ новые пол€ и их надо тоже как-то инитить, но без изменени€ самой верхнеуровневой абстракции это сделать нельз€ (ну или € об этом не знаю)
}
