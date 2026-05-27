using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProtectionColliders : MonoBehaviour
{
    private const string SWORD_TAG = "PlayerSword";

    [SerializeField] private Collider shieldCollider;

    private int hitCounter = 0;

    [SerializeField] private UnityEvent shieldActivated;
    [SerializeField] private UnityEvent staggerActvated;
    private void Start()
    {
        DisableCollider();
    }
    public void DisableCollider()
    {
        shieldCollider.enabled = false;
    }
    public void EnableCollider()
    {
        shieldCollider.enabled = true;
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (hitCounter < 15)
        {
            if (!collider.CompareTag(SWORD_TAG))
            {
                return;
            }
            shieldActivated.Invoke();

            hitCounter += 1;
            if (hitCounter == 15)
            {
                staggerActvated.Invoke();
            }
        }
    }
    public void RefreshHitCounter()
    {
        hitCounter = 0;
    }
}