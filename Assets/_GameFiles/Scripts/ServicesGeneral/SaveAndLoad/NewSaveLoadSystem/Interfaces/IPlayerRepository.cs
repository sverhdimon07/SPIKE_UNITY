public interface IPlayerRepository
{
    public void Save(PlayerSaveData data);

    public PlayerSaveData Load();
}
