namespace Core.Services.Retrieval;

public class SimpleListRetrieval<T> : ListRetrieval<T>
{
    private readonly IEnumerable<T> items;

    public SimpleListRetrieval(IEnumerable<T> items)
    {
        this.items = items;
    }

    public IEnumerable<T> Retrieve()
    {
        return items;
    }
}