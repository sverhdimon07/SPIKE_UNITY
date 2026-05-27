using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossHealthSystem : MonoBehaviour
{
    public static int counterBossLifeDestroyed = 0;
    public static float health = 100f;
    public static float secondHealth = 100f;
    public static float currentLivesCount = 3f;
    public static void HealthRegeneration()
    {
        health = 0;
        health += 100f;
        counterBossLifeDestroyed = 0;
    }
    public static void SecondHealthRegeneration()
    {
        secondHealth = 0;
        secondHealth += 100f;
    }

    public static void IncreaseBossLives()
    {
        if (currentLivesCount == 3f)
        {
            return;
        }
        currentLivesCount += 1;
    }
    public static void FillBossLives()
    {
        IncreaseBossLives();
        IncreaseBossLives();
        IncreaseBossLives();
    }
}