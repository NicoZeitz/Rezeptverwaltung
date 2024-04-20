namespace Core.Services.Retrieval;

public class OrderByProperty<T, K> : ListTransformer<T>
  where K : IComparable<K>
{
    public enum OrderDirection
    {
        Ascending,
        Descending
    };

    private readonly Func<T, K> keySelector;

    public OrderDirection OrderByDirection { get; init; } = OrderDirection.Ascending;

    public OrderByProperty(Func<T, K> keySelector, ListRetrieval<T> listRetrieval)
        : base(listRetrieval)
    {
        this.keySelector = keySelector;
    }

    public override IEnumerable<T> Transform(IEnumerable<T> items)
    {
        if (OrderByDirection == OrderDirection.Ascending)
        {
            return Order(items);
        }
        else
        {
            return OrderDescending(items);
        }
    }

    private IEnumerable<T> Order(IEnumerable<T> items)
    {
        return items.OrderBy(keySelector);
    }

    private IEnumerable<T> OrderDescending(IEnumerable<T> items)
    {
        return items.OrderByDescending(keySelector);
    }
}