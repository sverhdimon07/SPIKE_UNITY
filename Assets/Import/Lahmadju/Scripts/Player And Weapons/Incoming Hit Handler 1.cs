using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IncomingHitHandler1 : MonoBehaviour
{
    private const string RIGHTHAND_TAG = "PlayerRightHand";

    [SerializeField] private UnityEvent Handler1Activated;
    [SerializeField] private UnityEvent Handler1Deactivated;

    [SerializeField] private Collider triggerCollider;
    private void Start()
    {
        DisableHandlerCollider();
    }
    private void OnTriggerStay(Collider collider)
    {
        if (!collider.CompareTag(RIGHTHAND_TAG))
        {
            return;
        }
        Handler1Activated.Invoke();
    }
    private void OnTriggerExit(Collider collider)
    {
        if (!collider.CompareTag(RIGHTHAND_TAG))
        {
            return;
        }
        Handler1Deactivated.Invoke();
    }
    public void EnableHandlerCollider()
    {
        triggerCollider.enabled = true;
    }
    public void DisableHandlerCollider()
    {
        triggerCollider.enabled = false;

        Handler1Deactivated.Invoke();
    }
}