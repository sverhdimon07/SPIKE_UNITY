public interface ISceneSaveState
{
    bool HasFlightLimiter { get; }

    PlayerSaveData CapturePlayer(int regularEnemyScore);
    SavedCharactersData CaptureCharacters();
    bool HasReachedFlightLimiter(PlayerSaveData data);
    void RestorePlayer(PlayerSaveData data, bool reachedFlightLimiter);
    void RestoreEnemies(bool closeRange, bool longRange);
    void RestoreCharacters(SavedCharactersData characters);
}
