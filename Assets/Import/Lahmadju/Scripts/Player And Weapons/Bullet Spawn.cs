using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BulletSpawn : MonoBehaviour
{
    private const string LEFTHAND_TAG = "PlayerLeftHand";
    private const string RIGHTHAND_TAG = "PlayerRightHand";

    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawnPoint;
    private float BulletSpawnDelay = 0.75f;
    private float newTimeBulletSpawn = 0.0f;
    private bool gripActivated = false;

    private bool secondState = false;
    private bool cooldownActivated = false;

    [SerializeField] private InputActionProperty leftGripClick;
    [SerializeField] private InputActionProperty leftTriggerClick;
    [SerializeField] private InputActionProperty rightGripClick;
    [SerializeField] private InputActionProperty rightTriggerClick;

    [SerializeField] private UnityEvent bulletSpawned;
    [SerializeField] private UnityEvent bulletCantBeSpawned;
    private void OnTriggerStay(Collider collider)
    {
        if (!collider.CompareTag(LEFTHAND_TAG) && !collider.CompareTag(RIGHTHAND_TAG))
        {
            return;
        }
        if (collider.CompareTag(LEFTHAND_TAG))
        {
            if (leftGripClick.action.ReadValue<float>() > 0)
            {
                gripActivated = true;
            }
            if ((leftTriggerClick.action.ReadValue<float>() > 0) && (gripActivated == true))
            {
                if (secondState == false)
                {
                    BulletCreation();
                }
                else
                {
                    if (cooldownActivated == false)
                    {
                        bulletCantBeSpawned.Invoke();
                        Cooldown(1f);
                    }
                }
            }
        }
        if (collider.CompareTag(RIGHTHAND_TAG))
        {
            if (rightGripClick.action.ReadValue<float>() > 0)
            {
                gripActivated = true;
            }
            if ((rightTriggerClick.action.ReadValue<float>() > 0) && (gripActivated == true))
            {
                if (secondState == false)
                {
                    BulletCreation();
                }
                else
                {
                    if (cooldownActivated == false)
                    {
                        bulletCantBeSpawned.Invoke();
                        Cooldown(1f);
                    }
                }
            }
        }

    }
    private void BulletCreation()
    {
        if (Time.time < newTimeBulletSpawn)
        {
            return;
        }
        newTimeBulletSpawn = Time.time + BulletSpawnDelay;
        GameObject bulletCopy = Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        bulletSpawned.Invoke();
        bulletCopy.GetComponent<ConstantForce>().relativeForce = new Vector3(0, 0, 600);
        Destroy(bulletCopy, 3.0f);
    }
    private void Cooldown(float timeEnding)
    {
        cooldownActivated = true;
        Invoke(nameof(Make—ooldownActivatedFalse), timeEnding);
    }
    private void Make—ooldownActivatedFalse()
    {
        cooldownActivated = false;
    }
    public void EnableSecondState()
    {
        secondState = true;
    }
    public void DisableSecondState()
    {
        secondState = false;
    }
}