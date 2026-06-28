using System;
using UnityEngine;
using UnityEngine.Events;

public class ScoreController : MonoBehaviour
{
    public UnityAction BossSpawned;
    public static UnityAction ScoreIncreased;

    private CharacterControllerNewBoss _boss;

    private int _score;
    private int counter;

    public int Score => _score;

    private void Awake()
    {
        _score = 0;
        _boss = FindAnyObjectByType<CharacterControllerNewBoss>();
        _boss.gameObject.SetActive(false);
    }

    public void SetScore(int score)
    {
        _score = score;
    }

    public void IncreaseScore()
    {
        _score += 1;

        if (_score == 3)
        {
            _boss.gameObject.SetActive(true);
        }

        ScoreIncreased.Invoke();
    }
}
