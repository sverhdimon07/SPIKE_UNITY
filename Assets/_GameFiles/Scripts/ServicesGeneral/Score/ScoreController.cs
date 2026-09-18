using UnityEngine;
using UnityEngine.Events;

public class ScoreController : MonoBehaviour
{
    public static int Score;

    public static UnityAction ScoreIncreased;

    private CharacterControllerNewBoss _boss;

    private int _score;

    public UnityAction BossSpawned;

    //public int Score => _score;

    private void Awake()
    {
        _score = 0;
        Score = 0;
        _boss = FindAnyObjectByType<CharacterControllerNewBoss>();
        _boss.gameObject.SetActive(false);
    }

    public void SetScore(int score)
    {
        _score = score;
        Score = score;
    }

    public void IncreaseScore()
    {
        _score += 1;
        Score += 1;

        if (_score == 1)
        {
            _boss.gameObject.SetActive(true);
        }

        ScoreIncreased.Invoke();
    }
}
