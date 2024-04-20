namespace Core.Services.Retrieval;

public interface ListRetrieval<T>
{
    IEnumerable<T> Retrieve();
}