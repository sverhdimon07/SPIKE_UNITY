using UnityEngine;
using UnityEngine.Events;

public class ScoreController : MonoBehaviour
{
    public const int BossSpawnScore = 2;

    public static int Score;

    public static UnityAction ScoreChanged;

    public int RegularEnemyScore { get; private set; }

    private CharacterControllerNewBoss _boss;

    private int _score;

    public UnityAction BossSpawned;

    //public int Score => _score;

    private void Awake()
    {
        _score = 0;
        Score = 0;
        _boss = FindAnyObjectByType<CharacterControllerNewBoss>();
        UpdateBossVisibility();
    }

    public void SetScore(int score)
    {
        RegularEnemyScore = score;
        _score = score;
        Score = score;
        UpdateBossVisibility();
        ScoreChanged?.Invoke();
    }

    public void IncreaseRegularEnemyScore()
    {
        RegularEnemyScore += 1;
        IncreaseScore();
    }

    public void IncreaseScore()
    {
        _score += 1;
        Score += 1;

        UpdateBossVisibility();
        ScoreChanged?.Invoke();
    }

    private void UpdateBossVisibility()
    {
        if (_boss != null)
        {
            _boss.gameObject.SetActive(_score >= BossSpawnScore);
        }
    }
}
