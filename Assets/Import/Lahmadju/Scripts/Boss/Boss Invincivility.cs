using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossInvincivility : MonoBehaviour
{
    [SerializeField] private UnityEvent invincibilityEnabled;
    [SerializeField] private UnityEvent invincibilityDisabled;
    public void EnableInvincibility()
    {
        invincibilityEnabled.Invoke();
    }
    public void DisableInvincibility()
    {
        invincibilityDisabled.Invoke();
    }
}