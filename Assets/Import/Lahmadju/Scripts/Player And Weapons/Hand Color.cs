using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandColor : MonoBehaviour
{
    [SerializeField] private Material standartMaterial;

    [SerializeField] private SkinnedMeshRenderer handRender;

    public void ChangeColorForTime(Material material)
    {
        handRender.material = material;
        Invoke(nameof(ChangeColorOnStandart), 0.3f);
    }
    private void ChangeColorOnStandart()
    {
        handRender.material = standartMaterial;
    }
}