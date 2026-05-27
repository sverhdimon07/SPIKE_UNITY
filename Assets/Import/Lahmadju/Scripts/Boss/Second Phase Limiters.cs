using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondPhaseLimiters : MonoBehaviour
{
    [SerializeField] private BoxCollider[] limiters;
    private void Start()
    {
        DisableLimiters();
    }
    public void DisableLimiters()
    {
        foreach (BoxCollider collider in limiters)
        {
            collider.enabled = false;
        }
    }
    public void EnableLimiters()
    {
        foreach (BoxCollider collider in limiters)
        {
            collider.enabled = true;
        }
    }
}
