public class SavingLoadingPlayerInteractor : ISavingLoadingInteractor
{
    private ISavingLoadingRepository _savingLoadingRepository;

    public void Initialize(ISavingLoadingRepository savingLoadingRepository, string filePath) //хз, делать ли контракт на инит
    {
        savingLoadingRepository.Initialize(filePath);
    }
}
