using System;
using System.IO;

public sealed class GameSaveLoadInteractor
{
    private const string LongRangeSaveType = "CharacterControllerNewLongRange";

    private readonly IPlayerRepository _playerRepository;
    private readonly ICharacterRepository _characterRepository;
    private readonly ScoreController _scoreController;
    private readonly ISceneSaveState _scene;
    private readonly Action _reloadLevel;
    private readonly Action<Exception> _logException;
    private bool _hasWon;

    public GameSaveLoadInteractor(
        IPlayerRepository playerRepository,
        ICharacterRepository characterRepository,
        ScoreController scoreController,
        ISceneSaveState scene,
        Action reloadLevel,
        Action<Exception> logException)
    {
        _playerRepository = playerRepository;
        _characterRepository = characterRepository;
        _scoreController = scoreController;
        _scene = scene;
        _reloadLevel = reloadLevel;
        _logException = logException;
    }

    public void ReloadFromSave()
    {
        if (_playerRepository.Exists())
        {
            _reloadLevel();
        }
    }

    public void ClearSaveAfterVictory()
    {
        _hasWon = true;
        DeleteSaveFile(_playerRepository.Delete);
        DeleteSaveFile(_characterRepository.Delete);
    }

    private void DeleteSaveFile(Action delete)
    {
        try
        {
            delete();
        }
        catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException)
        {
            _logException(exception);
        }
    }

    public void ExecuteSaveOperation()
    {
        if (_hasWon)
        {
            return;
        }

        PlayerSaveData player = _scene.CapturePlayer(_scoreController.RegularEnemyScore);
        if (player != null)
        {
            _playerRepository.Save(player);
        }

        _characterRepository.Save(_scene.CaptureCharacters());
    }

    public void ExecuteLoadOperation()
    {
        PlayerSaveData player = _playerRepository.Load();
        if (player == null)
        {
            return;
        }

        SavedCharactersData characters = _characterRepository.Load();
        bool reachedFlightLimiter = _scene.HasReachedFlightLimiter(player);
        int score = !_scene.HasFlightLimiter || reachedFlightLimiter
            ? Math.Clamp(player.Score, 0, ScoreController.BossSpawnScore)
            : 0;

        _scene.RestorePlayer(player, reachedFlightLimiter);
        _scoreController.RestoreRegularEnemyScore(score);
        RestoreEnemies(reachedFlightLimiter, score, characters?.Characters);
        _scene.RestoreCharacters(characters);
    }

    private void RestoreEnemies(bool reachedFlightLimiter, int score, CharacterSaveData[] characters)
    {
        if (!_scene.HasFlightLimiter)
        {
            return;
        }

        bool beforeBoss = reachedFlightLimiter && score < ScoreController.BossSpawnScore;
        bool onlyLongRange = Array.Exists(characters ?? Array.Empty<CharacterSaveData>(),
            character => character?.CharacterType == LongRangeSaveType);

        _scene.RestoreEnemies(
            beforeBoss && (score == 0 || !onlyLongRange),
            beforeBoss && (score == 0 || onlyLongRange));
    }
}
