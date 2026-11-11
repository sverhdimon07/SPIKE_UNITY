using UnityEngine;
using System.Collections.Generic;

public class GameSaveLoadInteractor : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private FlightLimiter _flightLimiter;

    private IPlayerRepository _playerRepository;
    private ICharacterRepository _characterRepository;
    private Vector3 _levelStartPosition;

    private const string PLAYER_FILE = "player_save.json";
    private const string CHARACTER_FILE = "characters_save.json";

    private void Awake()
    {
        _playerRepository = new JsonPlayerRepository(PLAYER_FILE);
        _characterRepository = new JsonCharacterRepository(CHARACTER_FILE);

        Application.quitting += OnApplicationQuitting;
    }

    private void Start()
    {
        _levelStartPosition = _playerController.transform.position;
        ExecuteLoadOperation();
    }

    private void OnDestroy()
    {
        Application.quitting -= OnApplicationQuitting;
    }

    private void OnApplicationQuitting()
    {
        ExecuteSaveOperation();
    }

    public void ExecuteSaveOperation()
    {
        SavePlayer();
        SaveCharacters();
    }

    public void ExecuteLoadOperation()
    {
        PlayerSaveData player = _playerRepository.Load();
        if (player == null)
        {
            Debug.Log("No player save file found");
            return;
        }

        CharacterSaveData[] characters = _characterRepository.Load()?.Characters;
        bool reachedFlightLimiter = HasReachedFlightLimiter(player);
        int score = _flightLimiter == null || reachedFlightLimiter ? player.Score : 0;
        LoadPlayer(player, reachedFlightLimiter, score);
        RestoreEnemies(reachedFlightLimiter, score, characters);
        LoadCharacters(characters);
    }

    private void SavePlayer()
    {
        if (_playerController == null)
        {
            Debug.LogError("PlayerController missing");
            return;
        }

        var data = new PlayerSaveData
        {
            Position = _playerController.transform.position,
            Rotation = _playerController.transform.rotation,
            FormRotation = _playerController.FormRotation,
            Health = _playerController.Health,
            Score = ScoreController.Score,
            IsFighter = _playerController.IsFighter,
            IsFlightLocked = _playerController.IsFlightLocked,
            HasReachedFlightLimiter = _flightLimiter != null && _playerController.IsFlightLocked,
            IsFalling = _playerController.IsFalling,
            FallSpeed = _playerController.FallSpeed
        };

        _playerRepository.Save(data);
        Debug.Log("Player saved");
    }

    private bool HasReachedFlightLimiter(PlayerSaveData data)
    {
        return _flightLimiter != null && (data.HasReachedFlightLimiter ||
            (data.IsFlightLocked && _flightLimiter.IsAtOrPastBoundary(
                _levelStartPosition, data.Position, _playerController.GetComponent<CapsuleCollider>())));
    }

    private void LoadPlayer(PlayerSaveData data, bool reachedFlightLimiter, int score)
    {
        _playerController.SetRenderAndSkeletonPivot(data.Rotation);
        bool flightLocked = _flightLimiter == null ? data.IsFlightLocked : reachedFlightLimiter;
        _playerController.RestoreForm(data.Position, data.FormRotation, data.IsFighter, data.IsFalling, data.FallSpeed, flightLocked);
        _playerController.SetHealthValue(data.Health > 0f ? data.Health : 100f);
        _playerController.ScoreController.SetScore(score);
        Debug.Log("Player loaded");
    }

    private void RestoreEnemies(bool reachedFlightLimiter, int score, CharacterSaveData[] characters)
    {
        if (_flightLimiter == null)
        {
            return;
        }

        bool beforeBoss = reachedFlightLimiter && score < ScoreController.BossSpawnScore;
        bool onlyLongRange = System.Array.Exists(characters ?? System.Array.Empty<CharacterSaveData>(),
            character => character?.CharacterType == nameof(CharacterControllerNewLongRange));

        _flightLimiter.RestoreEnemies(
            beforeBoss && (score == 0 || !onlyLongRange),
            beforeBoss && (score == 0 || onlyLongRange));
    }

    private void SaveCharacters()
    {
        CharacterControllerNew[] allCharacters = FindObjectsByType<CharacterControllerNew>(FindObjectsSortMode.None);

        List<CharacterSaveData> charactersDataList = new List<CharacterSaveData>(allCharacters.Length);

        foreach (var character in allCharacters)
        {
            charactersDataList.Add(new CharacterSaveData
            {
                CharacterType = character.GetType().Name,
                Position = character.transform.position,
                Rotation = character.transform.rotation,
                Health = character.Health
            });
        }

        SavedCharactersData wrapper = new SavedCharactersData { Characters = charactersDataList.ToArray() };
        _characterRepository.Save(wrapper);
        Debug.Log($"Saved {charactersDataList.Count} characters");
    }

    private void LoadCharacters(CharacterSaveData[] characters)
    {
        if (characters == null)
        {
            Debug.Log("No characters save file found");
            return;
        }

        CharacterControllerNew[] currentCharacters = FindObjectsByType<CharacterControllerNew>(FindObjectsSortMode.None);

        var remaining = new List<CharacterSaveData>(characters);
        int count = 0;
        foreach (var character in currentCharacters)
        {
            string characterType = character.GetType().Name;
            int index = remaining.FindIndex(saved => saved?.CharacterType == characterType);
            if (index < 0)
            {
                index = remaining.FindIndex(saved => saved != null && string.IsNullOrEmpty(saved.CharacterType));
            }

            if (index < 0)
            {
                continue;
            }

            CharacterSaveData data = remaining[index];
            remaining.RemoveAt(index);
            character.RestoreSavedPosition(data.Position);
            character.SetRenderAndSkeletonPivot(data.Rotation);
            float maxHealth = character.Model.HealthController.Health.MaxHealthValue;
            character.SetHealthValue(data.Health > 0f ? Mathf.Min(data.Health, maxHealth) : maxHealth);
            count++;
        }

        Debug.Log($"Loaded {count} characters");
    }
}
