using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHoldingPointPosition : MonoBehaviour
{
    [SerializeField] Transform bossHoldingPoint;
    [SerializeField] Transform target;

    private float value = 0.63f;
    private void Start()
    {
        enabled = false;
    }
    private void Update()
    {
        bossHoldingPoint.position = Vector3.Lerp(transform.position, target.position, value);
    }
}