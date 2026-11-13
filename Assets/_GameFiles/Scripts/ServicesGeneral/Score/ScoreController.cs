using System;

public sealed class ScoreController
{
    public const int BossSpawnScore = 2;

    public event Action<int> ScoreChanged;

    public int RegularEnemyScore { get; private set; }
    public int TotalScore => RegularEnemyScore + _bossScore;

    public bool IsBossUnlocked => RegularEnemyScore >= BossSpawnScore;

    private int _bossScore;

    //public int Score => _score;

    public void RestoreRegularEnemyScore(int score)
    {
        RegularEnemyScore = score;
        _bossScore = 0;
        ScoreChanged?.Invoke(TotalScore);
    }

    public void RecordRegularEnemyDefeat()
    {
        RegularEnemyScore++;
        ScoreChanged?.Invoke(TotalScore);
    }

    public void RecordBossPhasePair()
    {
        _bossScore++;
        ScoreChanged?.Invoke(TotalScore);
    }

}
