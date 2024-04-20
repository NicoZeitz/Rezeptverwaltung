using Core.Entities;
using Core.Interfaces;
using Core.ValueObjects;

namespace Core.Services.Retrieval;

public class FilterAccessRights<T> : ListTransformer<T> where T : AccessRights
{
    private readonly Chef? viewer;

    public FilterAccessRights(Chef? viewer, ListRetrieval<T> listRetrieval) : base(listRetrieval)
    {
        this.viewer = viewer;
    }

    public override IEnumerable<T> Transform(IEnumerable<T> items)
    {
        if (viewer is null)
        {
            return FilterVisibleToGuest(items);
        }
        else
        {
            return FilterVisibleToViewer(items);
        }
    }

    private IEnumerable<T> FilterVisibleToGuest(IEnumerable<T> items)
    {
        return items.Where(item => item.Visibility.IsPublic());
    }

    private IEnumerable<T> FilterVisibleToViewer(IEnumerable<T> items)
    {
        return items.Where(item => item.Visibility.IsPublic() || item.IsVisibleTo(viewer!));
    }
}