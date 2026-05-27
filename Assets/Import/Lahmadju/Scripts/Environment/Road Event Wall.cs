using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoadEventWall : MonoBehaviour
{
    private const string PLAYER_TAG = "PlayerBox";

    [SerializeField] private BoxCollider mainDoorsCollider;

    [SerializeField] private UnityEvent wallActivated;
    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag(PLAYER_TAG))
        {
            return;
        }
        wallActivated.Invoke();
        Invoke(nameof(DisableIsTriggerMainDoorsCollider), 1f);
    }
    private void DisableIsTriggerMainDoorsCollider()
    {
        mainDoorsCollider.isTrigger = false;
    }
}