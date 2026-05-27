using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileLaunch : MonoBehaviour
{
    [SerializeField] private ForceMode forceMode = ForceMode.VelocityChange;

    [SerializeField] private float launchSpeed = 50f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Launch()
    {
        if (rb != null)
        {
            rb.AddForce((-transform.forward) * launchSpeed, forceMode);
        }
    }
}
