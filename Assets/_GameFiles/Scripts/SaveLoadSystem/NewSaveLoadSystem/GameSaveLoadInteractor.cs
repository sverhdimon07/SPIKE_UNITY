using UnityEngine;
using System.Collections.Generic;

public class GameSaveLoadInteractor : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private PlayerController _playerController;

    private IPlayerRepository _playerRepository;
    private ICharacterRepository _characterRepository;

    private const string PLAYER_FILE = "player_save.json";
    private const string CHARACTER_FILE = "characters_save.json";

    private void Awake()
    {
        _playerRepository = new JsonPlayerRepository(PLAYER_FILE);
        _characterRepository = new JsonCharacterRepository(CHARACTER_FILE);

        ExecuteLoadOperation();

        Application.quitting += OnApplicationQuitting;
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
        LoadPlayer();
        LoadCharacters();
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
            Health = _playerController.Health,
            //Score = _playerController.ScoreController.Score
        };

        _playerRepository.Save(data);
        Debug.Log("Player saved");
    }

    private void LoadPlayer()
    {
        var data = _playerRepository.Load();
        if (data != null)
        {
            _playerController.transform.position = data.Position;

            _playerController.SetLastPosition(data.Position);
            _playerController.SetRenderAndSkeletonPivot(data.Rotation);
            _playerController.SetHealthValue(data.Health);
            //_playerController.ScoreController.SetScore(data.Score);
            Debug.Log("Player loaded");
        }
        else
        {
            Debug.Log("No player save file found");
        }
    }

    private void SaveCharacters()
    {
        CharacterControllerNew[] allCharacters = FindObjectsByType<CharacterControllerNew>(FindObjectsSortMode.None);

        if (allCharacters.Length == 0)
        {
            Debug.Log("No characters found to save");
            return;
        }

        List<CharacterSaveData> charactersDataList = new List<CharacterSaveData>();

        foreach (var character in allCharacters)
        {
            charactersDataList.Add(new CharacterSaveData
            {
                Position = character.transform.position,
                Rotation = character.transform.rotation,
                Health = character.Health
            });
        }

        SavedCharactersData wrapper = new SavedCharactersData { Characters = charactersDataList.ToArray() };
        _characterRepository.Save(wrapper);
        Debug.Log($"Saved {charactersDataList.Count} characters");
    }

    private void LoadCharacters()
    {
        SavedCharactersData wrapper = _characterRepository.Load();
        if (wrapper == null || wrapper.Characters == null)
        {
            Debug.Log("No characters save file found");
            return;
        }

        CharacterControllerNew[] currentCharacters = FindObjectsByType<CharacterControllerNew>(FindObjectsSortMode.None);

        if (currentCharacters.Length != wrapper.Characters.Length)
        {
            Debug.LogWarning($"Number of characters changed: saved {wrapper.Characters.Length}, current {currentCharacters.Length}. Restoration may be incorrect.");
        }

        int count = Mathf.Min(currentCharacters.Length, wrapper.Characters.Length);
        for (int i = 0; i < count; i++)
        {
            currentCharacters[i].transform.position = wrapper.Characters[i].Position;
            currentCharacters[i].transform.rotation = wrapper.Characters[i].Rotation;
            currentCharacters[i].SetHealthValue(wrapper.Characters[i].Health);
        }

        Debug.Log($"Loaded {count} characters");
    }
}
