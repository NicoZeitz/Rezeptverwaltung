namespace Core.Services.Retrieval;

public class OptionalSingleItemListRetrieval<T> : ListRetrieval<T>
{
    private readonly T? item;

    public OptionalSingleItemListRetrieval(T? item)
    {
        this.item = item;
    }

    public IEnumerable<T> Retrieve()
    {
        if (item is null)
        {
            return Enumerable.Empty<T>();
        }

        return new List<T> { item };
    }
}