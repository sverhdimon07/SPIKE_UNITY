using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunColor : MonoBehaviour
{
    [SerializeField] private Material standartMaterial;

    [SerializeField] private MeshRenderer gunRender;

    public void ChangeColorForTime(Material material)
    {
        gunRender.material = material;
        Invoke(nameof(ChangeColorOnStandart), 0.3f);
    }
    private void ChangeColorOnStandart()
    {
        gunRender.material = standartMaterial;
    }
}
