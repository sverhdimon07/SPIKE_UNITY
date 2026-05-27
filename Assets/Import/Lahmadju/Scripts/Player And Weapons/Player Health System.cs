using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{
    public static float health = 100f;
    public static float currentLivesCount = 3f;
    public static void HealthRegeneration()
    {
        health = 0;
        health += 100f;
    }

    public static void IncreasePlayerLives()
    {
        if (currentLivesCount == 3f)
        {
            return;
        }
        currentLivesCount += 1;
    }
    public static void FillPlayerLives()
    {
        IncreasePlayerLives();
        IncreasePlayerLives();
        IncreasePlayerLives();
    }
}