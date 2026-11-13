using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class SceneSaveState : ISceneSaveState
{
    private readonly PlayerController _playerController;
    private readonly FlightLimiter _flightLimiter;
    private Vector3 _levelStartPosition;

    public SceneSaveState(PlayerController playerController, FlightLimiter flightLimiter)
    {
        _playerController = playerController;
        _flightLimiter = flightLimiter;
    }

    public bool HasFlightLimiter => _flightLimiter != null;

    public void CaptureLevelStart()
    {
        _levelStartPosition = _playerController.transform.position;
    }

    public PlayerSaveData CapturePlayer(int regularEnemyScore)
    {
        if (_playerController == null)
        {
            Debug.LogError("PlayerController missing");
            return null;
        }

        var data = new PlayerSaveData
        {
            Position = _playerController.transform.position,
            Rotation = _playerController.transform.rotation,
            FormRotation = _playerController.FormRotation,
            Health = _playerController.Health,
            Score = regularEnemyScore,
            IsFighter = _playerController.IsFighter,
            IsFlightLocked = _playerController.IsFlightLocked,
            HasReachedFlightLimiter = _flightLimiter != null && _playerController.IsFlightLocked,
            IsFalling = _playerController.IsFalling,
            FallSpeed = _playerController.FallSpeed,
            IsKnifeEquipped = _playerController.IsKnifeEquipped,
            IsPistolEquipped = _playerController.IsPistolEquipped
        };

        return data;
    }

    public bool HasReachedFlightLimiter(PlayerSaveData data)
    {
        return _flightLimiter != null && (data.HasReachedFlightLimiter ||
            (data.IsFlightLocked && _flightLimiter.IsAtOrPastBoundary(
                _levelStartPosition, data.Position, _playerController.GetComponent<CapsuleCollider>())));
    }

    public void RestorePlayer(PlayerSaveData data, bool reachedFlightLimiter)
    {
        _playerController.SetRenderAndSkeletonPivot(data.Rotation);
        bool flightLocked = _flightLimiter == null ? data.IsFlightLocked : reachedFlightLimiter;
        _playerController.RestoreForm(data.Position, data.FormRotation, data.IsFighter, data.IsFalling, data.FallSpeed, flightLocked);
        _playerController.RestoreWeaponSelection(data.IsKnifeEquipped, data.IsPistolEquipped);
        var playerCollider = _playerController.GetComponent<CapsuleCollider>();
        foreach (var pickup in UnityEngine.Object.FindObjectsByType<PlayerWeaponPickup>(FindObjectsSortMode.None))
        {
            pickup.IgnorePlayerUntilExitIfOverlapping(playerCollider);
        }

        _playerController.SetHealthValue(data.Health > 0f ? data.Health : 100f);
        Debug.Log("Player loaded");
    }

    public void RestoreEnemies(bool closeRange, bool longRange)
    {
        _flightLimiter.RestoreEnemies(closeRange, longRange);
    }

    public SavedCharactersData CaptureCharacters()
    {
        CharacterControllerNew[] allCharacters = UnityEngine.Object.FindObjectsByType<CharacterControllerNew>(FindObjectsSortMode.None);

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
        return wrapper;
    }

    public void RestoreCharacters(SavedCharactersData characters)
    {
        if (characters == null)
        {
            Debug.Log("No characters save file found");
            return;
        }

        CharacterControllerNew[] currentCharacters = UnityEngine.Object.FindObjectsByType<CharacterControllerNew>(FindObjectsSortMode.None);

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
