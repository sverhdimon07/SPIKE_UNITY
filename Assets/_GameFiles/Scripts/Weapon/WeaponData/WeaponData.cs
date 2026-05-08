using UnityEngine;

[System.Serializable]
public sealed class WeaponData //проблема этого решения только в том, что WeaponData будет распухать, и по идее же у новых дочерних абстракций должно быть все больше и больше специализированных полей
{
    [Range(1f,100f)]
    [SerializeField] private float _damage;
    [Range(0.1f, 20f)]
    [SerializeField] private float _range;

    public float Damage => _damage;
    public float Range => _range;
}
