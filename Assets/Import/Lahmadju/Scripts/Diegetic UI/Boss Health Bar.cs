using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Image secondPhaseHealthBar;
    [SerializeField] private Image livesCounterBar1;
    [SerializeField] private Image livesCounterBar2;
    [SerializeField] private Image livesCounterBar3;
    private float lastLivesCount = 3f;
    public void RefreshHealthBarAndLives()
    {
        healthBar.fillAmount = (BossHealthSystem.health) / 100;
        if (lastLivesCount != BossHealthSystem.currentLivesCount)
        {
            LivesCounterChecking();
            lastLivesCount = BossHealthSystem.currentLivesCount;
        }
    }
    public void RefreshsecondPhaseHealthBar()
    {
        secondPhaseHealthBar.fillAmount = (BossHealthSystem.secondHealth) / 100;
    }
    private void LivesCounterChecking()
    {
        if (BossHealthSystem.currentLivesCount == 3f)
        {
            livesCounterBar1.fillAmount = 1;
            livesCounterBar2.fillAmount = 1;
            livesCounterBar3.fillAmount = 1;
            return;
        }
        if (BossHealthSystem.currentLivesCount == 2f)
        {
            livesCounterBar1.fillAmount = 1;
            livesCounterBar2.fillAmount = 1;
            livesCounterBar3.fillAmount = 0;
            return;
        }
        if (BossHealthSystem.currentLivesCount == 1f)
        {
            livesCounterBar1.fillAmount = 1;
            livesCounterBar2.fillAmount = 0;
            livesCounterBar3.fillAmount = 0;
            return;
        }
        if (BossHealthSystem.currentLivesCount == 0f)
        {
            livesCounterBar1.fillAmount = 0;
            livesCounterBar2.fillAmount = 0;
            livesCounterBar3.fillAmount = 0;
            return;
        }
    }
}