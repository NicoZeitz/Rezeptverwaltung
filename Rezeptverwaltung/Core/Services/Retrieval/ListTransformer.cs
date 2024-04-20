
namespace Core.Services.Retrieval;

public abstract class ListTransformer<T> : ListRetrieval<T>
{
    protected readonly ListRetrieval<T> listRetrieval;

    public ListTransformer(ListRetrieval<T> listRetrieval)
    {
        this.listRetrieval = listRetrieval;
    }

    public IEnumerable<T> Retrieve()
    {
        return Transform(listRetrieval.Retrieve());
    }

    public abstract IEnumerable<T> Transform(IEnumerable<T> items);
}