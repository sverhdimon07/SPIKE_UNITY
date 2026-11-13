using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSaveLoadInteractor : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private FlightLimiter _flightLimiter;

    private IPlayerRepository _playerRepository;
    private ICharacterRepository _characterRepository;
    private Vector3 _levelStartPosition;
    private bool _hasWon;

    private const string PLAYER_FILE = "player_save.json";
    private const string CHARACTER_FILE = "characters_save.json";

    private void Awake()
    {
        _playerRepository = new JsonPlayerRepository(PLAYER_FILE);
        _characterRepository = new JsonCharacterRepository(CHARACTER_FILE);
    }

    private void Start()
    {
        _levelStartPosition = _playerController.transform.position;
        ExecuteLoadOperation();
    }

    public void ReloadFromSave()
    {
        if (_playerRepository.Exists())
        {
            SceneLoading.LoadLevelScene();
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
            Debug.LogException(exception, this);
        }
    }

    public void ExecuteSaveOperation()
    {
        if (_hasWon)
        {
            return;
        }

        SavePlayer();
        SaveCharacters();
    }

    private void ExecuteLoadOperation()
    {
        PlayerSaveData player = _playerRepository.Load();
        if (player == null)
        {
            Debug.Log("No player save file found");
            return;
        }

        SavedCharactersData characters = _characterRepository.Load();
        bool reachedFlightLimiter = HasReachedFlightLimiter(player);
        int score = _flightLimiter == null || reachedFlightLimiter
            ? Mathf.Clamp(player.Score, 0, ScoreController.BossSpawnScore)
            : 0;
        LoadPlayer(player, reachedFlightLimiter, score);
        RestoreEnemies(reachedFlightLimiter, score, characters?.Characters);
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
            Score = _playerController.ScoreController.RegularEnemyScore,
            IsFighter = _playerController.IsFighter,
            IsFlightLocked = _playerController.IsFlightLocked,
            HasReachedFlightLimiter = _flightLimiter != null && _playerController.IsFlightLocked,
            IsFalling = _playerController.IsFalling,
            FallSpeed = _playerController.FallSpeed,
            IsKnifeEquipped = _playerController.IsKnifeEquipped,
            IsPistolEquipped = _playerController.IsPistolEquipped
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
        _playerController.RestoreWeaponSelection(data.IsKnifeEquipped, data.IsPistolEquipped);
        var playerCollider = _playerController.GetComponent<CapsuleCollider>();
        foreach (var pickup in FindObjectsByType<PlayerWeaponPickup>(FindObjectsSortMode.None))
        {
            pickup.IgnorePlayerUntilExitIfOverlapping(playerCollider);
        }

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
        bool onlyLongRange = Array.Exists(characters ?? Array.Empty<CharacterSaveData>(),
            character => character?.CharacterType == nameof(CharacterControllerNewLongRange));

        _flightLimiter.RestoreEnemies(
            beforeBoss && (score == 0 || !onlyLongRange),
            beforeBoss && (score == 0 || onlyLongRange));
    }

    private void SaveCharacters()
    {
        CharacterControllerNew[] allCharacters = FindObjectsByType<CharacterControllerNew>(FindObjectsSortMode.None);

        List<CharacterSaveData> charactersDataList = new List<CharacterSaveData>(allCharacters.Length);
        var wrapper = new SavedCharactersData();

        foreach (var character in allCharacters)
        {
            if (character is CharacterControllerNewBoss)
            {
                wrapper.Boss = new CharacterPositionData
                {
                    CharacterType = character.GetType().Name,
                    Position = character.transform.position,
                    Rotation = character.transform.rotation
                };
                continue;
            }

            charactersDataList.Add(new CharacterSaveData
            {
                CharacterType = character.GetType().Name,
                Position = character.transform.position,
                Rotation = character.transform.rotation,
                Health = character.Health
            });
        }

        wrapper.Characters = charactersDataList.ToArray();
        _characterRepository.Save(wrapper);
        Debug.Log($"Saved {allCharacters.Length} characters");
    }

    private void LoadCharacters(SavedCharactersData characters)
    {
        if (characters == null)
        {
            Debug.Log("No characters save file found");
            return;
        }

        CharacterControllerNew[] currentCharacters = FindObjectsByType<CharacterControllerNew>(FindObjectsSortMode.None);

        var remaining = new List<CharacterSaveData>(characters.Characters ?? Array.Empty<CharacterSaveData>());
        foreach (var character in currentCharacters)
        {
            string characterType = character.GetType().Name;
            if (character is CharacterControllerNewBoss && characters.Boss?.CharacterType == characterType)
            {
                RestoreCharacterPosition(character, characters.Boss);
                continue;
            }

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
            RestoreCharacterPosition(character, data);
            if (character is not CharacterControllerNewBoss)
            {
                float maxHealth = character.Model.HealthController.Health.MaxHealthValue;
                character.SetHealthValue(data.Health > 0f ? Mathf.Min(data.Health, maxHealth) : maxHealth);
            }
        }
    }

    private static void RestoreCharacterPosition(CharacterControllerNew character, CharacterPositionData data)
    {
        character.RestoreSavedPosition(data.Position);
        character.SetRenderAndSkeletonPivot(data.Rotation);
    }
}
