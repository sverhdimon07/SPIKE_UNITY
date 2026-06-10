using System;

public class Interactor<T> : IInteractor<T> where T : class
{
    private readonly IRepository<T> _repository;

    public Interactor(IRepository<T> repository)
    {
        _repository = repository;
    }

    public void Save(T data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        _repository.Save(data);
    }

    public T Load()
    {
        return _repository.Load();
    }
}
