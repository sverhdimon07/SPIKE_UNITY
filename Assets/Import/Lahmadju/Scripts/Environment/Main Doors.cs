using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainDoors : MonoBehaviour
{
    private const string PLAYER_TAG = "PlayerBox";

    [SerializeField] private Transform door1;
    [SerializeField] private Transform door2;

    private bool enterState = false;
    private bool exitState = true;

    private float speed1 = 0.01f;
    private float speed2 = 0.1f;

    private Vector3 destination1;
    private Vector3 destination2;
    private Vector3 destination3;
    private Vector3 destination4;

    [SerializeField] private UnityEvent opened;
    [SerializeField] private UnityEvent closed;
    private void Start()
    {
        destination1 = door1.position + new Vector3(3f, 0, 0);
        destination2 = door2.position - new Vector3(3f, 0, 0);
        destination3 = door1.position;
        destination4 = door2.position;
        enabled = false;
    }
    private void Update()
    {
        if (enterState == true)
        {
            OpenDoors(speed1);
        }
        else if (exitState == true)
        {
            CloseDoors(speed2);
        }
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag(PLAYER_TAG))
        {
            return;
        }
        opened.Invoke();
        enterState = true;
        exitState = false;
        Invoke(nameof(MakeEnabled), 0.8f);
    }
    private void OnTriggerExit(Collider collider)
    {
        if (!collider.CompareTag(PLAYER_TAG))
        {
            return;
        }
        closed.Invoke();
        enterState = false;
        exitState = true;
        Invoke(nameof(MakeDisabled), 1.5f);
    }
    private void OpenDoors(float speed)
    {
        door1.position = Vector3.Lerp(door1.position, destination1, speed);
        door2.position = Vector3.Lerp(door2.position, destination2, speed);
    }
    private void CloseDoors(float speed)
    {
        door1.position = Vector3.Lerp(door1.position, destination3, speed);
        door2.position = Vector3.Lerp(door2.position, destination4, speed);
    }
    private void MakeEnabled()
    {
        enabled = true;
    }
    private void MakeDisabled()
    {
        enabled = false;
    }
}