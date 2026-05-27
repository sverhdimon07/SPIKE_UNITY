using System;
using System.Collections;
using System.Collections.Generic;
//using Pathfinding;
using UnityEngine;
using UnityEngine.Events;

public class BossAI : MonoBehaviour
{
    [SerializeField] private BossAnimator bossAnimator;

    //[SerializeField] private AIDestinationSetter destinationSetter;

    [SerializeField] private Transform roamTarget;

    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;

    private Vector3 roamPosition;

    [SerializeField] private Transform bossTransform;
    [SerializeField] private Transform playerForBossChangeTransform;
    [SerializeField] private Transform playerTransform;
    //private Player playerEmptyComponent => FindObjectOfType<Player>();
    private Player playerEmptyComponent;

    [SerializeField] private BossInvincivility bossInvincivility;

    private bool cooldownActivated = false;

    private float enemyAtackRange = 4.5f;

    private bool roamingState = false;
    private bool followingState = false;
    private bool notFollowingState = false;
    private bool bullethellState = false;
    private bool staggerState = false;
    private bool deathState = false;

    private bool protectionState = false;
    private bool openingState = false;


    [SerializeField] private UnityEvent protectionActivated;
    [SerializeField] private UnityEvent openingActivated;
    [SerializeField] private UnityEvent staggerEnded;
    [SerializeField] private UnityEvent attackActivated;

    private int protectionStateFirstEntryCheck = 0;
    private int staggerStateEntryCounter = 0;
    private void Start()
    {
        //playerEmptyComponent = FindObjectOfType<Player>(); // ŒÕ‘À» “ — SPIKE

        roamingState = true;
        followingState = false;

        roamPosition = point1.position;

        bossAnimator.ControlWalkingAnimation(true);
    }
    private void Update()
    {
        if (roamingState == true)
        {
            RoamingStateLogic();
        }
        else if (followingState == true)
        {
            FollowingStateLogic();
        }
        else if (notFollowingState == true)
        {
            NotFollowingStateLogic();
        }
        else if (bullethellState == true)
        {

        }
        else if (staggerState == true)
        {
            if (staggerStateEntryCounter == 0)
            {
                staggerStateEntryCounter += 1;
                StaggerState();
            }
        }
        else if (deathState == true)
        {
            return;
        }
    }
    private void RoamingStateLogic()
    {
        roamTarget.position = roamPosition;
        if (Vector3.Distance(gameObject.transform.position, roamPosition) <= 1f)
        {
            if (roamPosition == point1.position)
            {
                roamPosition = point2.position;
            }
            else if (roamPosition == point2.position)
            {
                roamPosition = point1.position;
            }
        }
        //destinationSetter.target = roamTarget;
    }
    private void FollowingStateLogic()
    {
        if (Vector3.Distance(gameObject.transform.position, playerTransform.position) <= enemyAtackRange)
        {
            Vector3 playerTrackerPosition = new Vector3(playerTransform.position.x, bossTransform.position.y, playerTransform.position.z);
            bossTransform.LookAt(playerTrackerPosition);
            //destinationSetter.target = null;

            if (protectionStateFirstEntryCheck == 0)
            {
                protectionStateFirstEntryCheck = 1;
                ProtectionState();
            }

            if ((protectionState == true) && (openingState == false))
            {
                ProtectionState();
            }
            TryAttackPlayer();
        }
        else
        {
            protectionStateFirstEntryCheck = 0;
            bossAnimator.ControlBlockAnimation(false);
            bossAnimator.ControlWalkingAnimation(true);
            //destinationSetter.target = playerEmptyComponent.transform; // ŒÕ‘À» “ — SPIKE
        }
    }
    private void NotFollowingStateLogic()
    {
        Vector3 playerTrackerPosition = new Vector3(playerTransform.position.x, bossTransform.position.y, playerTransform.position.z);
        bossTransform.LookAt(playerTrackerPosition);

        ProtectionState();
        TryAttackPlayer();
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
    private void TryAttackPlayer()
    {
        if (cooldownActivated == false)
        {
            attackActivated.Invoke();

            bossAnimator.ControlWalkingAnimation(false);

            bossAnimator.PlayRandomAttack();
            Cooldown(5f);
        }
    }
    public void ProtectionState()
    {
        staggerState = false;

        openingState = false;
        protectionState = true;

        bossInvincivility.EnableInvincibility();

        bossAnimator.ControlWalkingAnimation(false);
        bossAnimator.ControlBlockAnimation(true);

        protectionActivated.Invoke();
    }
    public void OpeningState()
    {
        protectionState = false;
        openingState = true;

        bossInvincivility.DisableInvincibility();

        openingActivated.Invoke();
    }
    public void StaggerState()
    {
        OpeningState();
        bossAnimator.PlayStagger();
    }
    public void EndStaggerActions()
    {
        if (BossHealthSystem.counterBossLifeDestroyed == 0)
        {
            followingState = true;

            ProtectionState();

            staggerStateEntryCounter = 0;
            staggerEnded.Invoke();

            Cooldown(4f);
        }
        else
        {
            notFollowingState = true;

            ProtectionState();

            staggerStateEntryCounter = 0;
            staggerEnded.Invoke();

            Cooldown(4f);
        }
    }
    public void MakeStaggerStateTrue()
    {
        roamingState = false;
        followingState = false;
        notFollowingState = false;
        bullethellState = false;
        deathState = false;

        staggerState = true;
    }
    public void MakingNotFollowingStateTrue()
    {
        roamingState = false;
        followingState = false;
        bullethellState = false;
        deathState = false;

        notFollowingState = true;
    }
    public void MakingNotFollowingStateFalse()
    {
        roamingState = false;
        followingState = true;
        notFollowingState = false;
    }
    public void TryFindPlayer()
    {
        roamingState = false;
        notFollowingState = false;
        bullethellState = false;

        protectionState = false;
        openingState = false;

        followingState = true;
    }
    public void ActivateDeathState()
    {
        roamingState = false;
        followingState = false;
        notFollowingState = false;
        bullethellState = false;

        protectionState = false;
        openingState = false;

        deathState = true;
    }
    public void ChangeBossPositionToPlayer()
    {
        bossTransform.position = playerForBossChangeTransform.position;
    }
}