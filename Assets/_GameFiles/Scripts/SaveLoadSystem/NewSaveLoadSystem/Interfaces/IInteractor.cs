public interface IInteractor<T> where T : class
{
    public void Save(T data);

    public T Load();
}
