using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAcceptIncomingDamage : MonoBehaviour
{
    private const string SWORD_TAG = "BossSword";

    private float swordDamage = 15f; //по стандарту 20

    private float staggerExitTime = 7f;

    private bool staggerState = false;
    private bool playerWasKilledState = false;

    private bool playerInvincibility = false;

    [SerializeField] private UnityEvent playerDamaged;
    [SerializeField] private UnityEvent lifeDestroyed;
    [SerializeField] private UnityEvent regenerationEnded;
    [SerializeField] private UnityEvent playerDied;
    private void AcceptSwordDamage()
    {
        PlayerHealthSystem.health -= swordDamage;
        playerDamaged.Invoke();

        if (PlayerHealthSystem.health <= 0)
        {
            PlayerHealthSystem.currentLivesCount -= 1;
            lifeDestroyed.Invoke();

            if (PlayerHealthSystem.currentLivesCount == 0f)
            {
                playerWasKilledState = true;
                playerDied.Invoke();
                return;
            }
            staggerState = true;
            StaggerExit(staggerExitTime);
            return;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (playerWasKilledState == false)
        {
            if (staggerState == false)
            {
                if (!collider.CompareTag(SWORD_TAG))
                {
                    return;
                }
                if (playerInvincibility == false)
                {
                    AcceptSwordDamage();
                }
            }
        }
    }
    public void EnableInvincibility()
    {
        playerInvincibility = true;
    }
    public void DisableInvincibility()
    {
        playerInvincibility = false;
    }
    private void Regeneration()
    {
        PlayerHealthSystem.HealthRegeneration();
    }
    private void MakingStaggerFalse()
    {
        staggerState = false;
    }
    private void StaggerExit(float time)
    {
        Invoke(nameof(MakingStaggerFalse), time);
        Invoke(nameof(Regeneration), time);
        Invoke(nameof(InvokingRegenerationEndedEvent), time);

    }
    private void InvokingRegenerationEndedEvent()
    {
        regenerationEnded.Invoke();
    }
}