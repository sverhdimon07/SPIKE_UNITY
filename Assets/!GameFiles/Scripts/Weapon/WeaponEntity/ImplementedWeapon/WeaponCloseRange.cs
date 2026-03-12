using UnityEngine;

public class WeaponCloseRange : Weapon
{
    [Range(1f, 10f)]
    [SerializeField] private float _range; //пока пусть будет SerializeField

    public float Range => _range;
}
