using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSpeedController : MonoBehaviour
{
    private const string PLAYER_TAG = "PlayerBox";

    [SerializeField] private Transform objTransform;
    [SerializeField] private Transform bossTransform;

    [SerializeField] private UnityEvent speedDecreased;
    [SerializeField] private UnityEvent speedIncreased;
    private void Update()
    {
        objTransform.position = bossTransform.position;
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag(PLAYER_TAG))
        {
            return;
        }
        speedDecreased.Invoke();
    }
    private void OnTriggerExit(Collider collider)
    {
        if (!collider.CompareTag(PLAYER_TAG))
        {
            return;
        }
        speedIncreased.Invoke();
    }
}