using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncomingHitHandlerPosition : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform handlerTransform;
    private void Update()
    {
        Vector3 newRotation = cameraTransform.rotation.eulerAngles;
        Vector3 newPosition = cameraTransform.position;

        handlerTransform.rotation = Quaternion.Euler(0, newRotation.y, 0);
        handlerTransform.position = newPosition;
    }
}