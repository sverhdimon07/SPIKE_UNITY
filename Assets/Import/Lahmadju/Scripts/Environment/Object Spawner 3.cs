using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner3 : MonoBehaviour
{
    [SerializeField] private GameObject spawningObject;
    [SerializeField] private GameObject spawningObjectRotationPivot;

    private Vector3 spawnPoint;
    private void Awake()
    {
        enabled = false;
    }
    private void Start()
    {
        spawnPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        for (int i = 0; i < 3; i++)
        {
            Invoke(nameof(ObjectCreation), (i + 1f) / 3f);
        }
    }
    private void ObjectCreation()
    {
        GameObject objectCopy = Instantiate(spawningObject, spawnPoint, spawningObjectRotationPivot.transform.rotation);
        objectCopy.GetComponent<ConstantForce>().relativeForce = new Vector3(0, 0, -7000);
        Destroy(objectCopy, 10f);
    }
}