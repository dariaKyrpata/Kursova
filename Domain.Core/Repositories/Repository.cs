using Domain.Core.Interfaces;

namespace Domain.Core.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly List<T> _items = new();

    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items.Add(item);
    }

    public bool Remove(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return _items.Remove(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(_items);
    }

    public List<T> Find(Predicate<T> match)
    {
        ArgumentNullException.ThrowIfNull(match);
        return _items.FindAll(match);
    }

    public void Clear()
    {
        _items.Clear();
    }
}