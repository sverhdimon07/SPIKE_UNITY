using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    /*
    [Range(1f, 100f)]
    [SerializeField] private float _damage; //пока пусть будет SerializeField
    */
    private float _damage;

    public float Damage => _damage;

    public void Initialize(float damage)
    {
        _damage = damage;
    }
}
