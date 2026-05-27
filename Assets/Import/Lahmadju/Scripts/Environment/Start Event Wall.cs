using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartEventWall : MonoBehaviour
{
    private const string PLAYER_TAG = "PlayerBox";

    [SerializeField] private UnityEvent wallActivated;
    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag(PLAYER_TAG))
        {
            return;
        }
        wallActivated.Invoke();
    }
}
