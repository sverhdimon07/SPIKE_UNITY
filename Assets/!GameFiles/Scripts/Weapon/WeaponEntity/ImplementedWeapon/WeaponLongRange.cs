using UnityEngine;

public class WeaponLongRange
{
    [Range(10f, 100f)]
    [SerializeField] private float _range; //пока пусть будет SerializeField

    public float Range => _range;
}
