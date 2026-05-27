using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IncomingHitHandler4 : MonoBehaviour
{
    private const string RIGHTHAND_TAG = "PlayerRightHand";

    [SerializeField] private UnityEvent Handler4Activated;
    [SerializeField] private UnityEvent Handler4Deactivated;

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
        Handler4Activated.Invoke();
    }
    private void OnTriggerExit(Collider collider)
    {
        if (!collider.CompareTag(RIGHTHAND_TAG))
        {
            return;
        }
        Handler4Deactivated.Invoke();
    }
    public void EnableHandlerCollider()
    {
        triggerCollider.enabled = true;
    }
    public void DisableHandlerCollider()
    {
        triggerCollider.enabled = false;

        Handler4Deactivated.Invoke();
    }
}