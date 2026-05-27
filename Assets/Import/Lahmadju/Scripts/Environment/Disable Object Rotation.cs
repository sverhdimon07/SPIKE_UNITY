using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObjectRotation : MonoBehaviour
{
    [SerializeField] private Transform obj;
    private void Start()
    {
        enabled = false;
    }
    private void Update()
    {
        obj.rotation = Quaternion.identity;
    }
}