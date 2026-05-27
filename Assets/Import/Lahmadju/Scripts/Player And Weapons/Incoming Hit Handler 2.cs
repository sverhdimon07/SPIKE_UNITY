using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IncomingHitHandler2 : MonoBehaviour
{
    private const string RIGHTHAND_TAG = "PlayerRightHand";

    [SerializeField] private UnityEvent Handler2Activated;
    [SerializeField] private UnityEvent Handler2Deactivated;

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
        Handler2Activated.Invoke();
    }
    private void OnTriggerExit(Collider collider)
    {
        if (!collider.CompareTag(RIGHTHAND_TAG))
        {
            return;
        }
        Handler2Deactivated.Invoke();
    }
    public void EnableHandlerCollider()
    {
        triggerCollider.enabled = true;
    }
    public void DisableHandlerCollider()
    {
        triggerCollider.enabled = false;

        Handler2Deactivated.Invoke();
    }
}