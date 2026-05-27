using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossLegsAndTorsoAction : MonoBehaviour
{
    private const string SWORD_TAG = "PlayerSword";

    [SerializeField] private GameObject torso;
    [SerializeField] private GameObject leftLeg;
    [SerializeField] private GameObject rightLeg;
    private float power = 50;

    [SerializeField] private UnityEvent limbCut;
    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag(SWORD_TAG))
        {
            return;
        }
        gameObject.AddComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * power);
        if ((leftLeg.GetComponent<Rigidbody>().isKinematic == false) && (rightLeg.GetComponent<Rigidbody>().isKinematic == false))
        {
            torso.AddComponent<Rigidbody>().isKinematic = false;
            limbCut.Invoke();
        }
    }
}