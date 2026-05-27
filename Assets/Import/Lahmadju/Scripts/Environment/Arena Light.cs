using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class ArenaLight : MonoBehaviour
{
    [SerializeField] private Light lightObject;

    private float intensityDelay = 0.04f;
    public void OnWallActivated()
    {
        for (int i = 0; i < 15; i++)
        {
            Invoke(nameof(IncreasingIntensity), intensityDelay * i);
        }
    }
    private void IncreasingIntensity()
    {
        lightObject.intensity += 1;
    }
}