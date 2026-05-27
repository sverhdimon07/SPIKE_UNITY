using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IncomingHitHandler3 : MonoBehaviour
{
    private const string RIGHTHAND_TAG = "PlayerRightHand";

    [SerializeField] private UnityEvent Handler3Activated;
    [SerializeField] private UnityEvent Handler3Deactivated;

    [SerializeField] private Collider triggerCollider;
    private void Start()
    {
        DisableHandlerCollider();
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag(RIGHTHAND_TAG))
        {
            return;
        }
        Handler3Activated.Invoke();
    }
    private void OnTriggerExit(Collider collider)
    {
        if (!collider.CompareTag(RIGHTHAND_TAG))
        {
            return;
        }
        Handler3Deactivated.Invoke();
    }
    public void EnablerHandlerCollider()
    {
        triggerCollider.enabled = true;
    }
    public void DisableHandlerCollider()
    {
        triggerCollider.enabled = false;

        Handler3Deactivated.Invoke();
    }
}