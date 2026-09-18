public interface ICharacterRepository
{
    public void Save(SavedCharactersData data);

    public SavedCharactersData Load();
}
