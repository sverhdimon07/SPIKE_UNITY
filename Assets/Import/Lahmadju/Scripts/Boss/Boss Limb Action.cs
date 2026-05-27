using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossLimbAction : MonoBehaviour
{
    private const string SWORD_TAG = "PlayerSword";
    
    private float power = 100;

    [SerializeField] private UnityEvent headCut;
    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag(SWORD_TAG))
        {
            return;
        }
        gameObject.AddComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * power);

        headCut.Invoke();
    }
}