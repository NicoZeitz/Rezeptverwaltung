using Core.Entities;
using Core.Repository;
using Core.Services.Retrieval;
using Core.ValueObjects;

namespace Core.Services;

public class ShowCookbooks
{
    private readonly CookbookRepository cookbookRepository;

    public ShowCookbooks(CookbookRepository cookbookRepository) : base()
    {
        this.cookbookRepository = cookbookRepository;
    }

    public Cookbook? ShowSingleCookbook(Identifier identifier, Chef? viewer)
    {
        var retrievalGraph = CreateRetrievalGraph(
            viewer,
            new OptionalSingleItemListRetrieval<Cookbook>(
                cookbookRepository.FindByIdentifier(identifier)
            )
        );
        return retrievalGraph
            .Retrieve()
            .FirstOrDefault();
    }

    public IEnumerable<Cookbook> ShowCookbooksForChef(Chef chef, Chef? viewer)
    {
        var retrievalGraph = CreateRetrievalGraph(
            viewer,
            new SimpleListRetrieval<Cookbook>(
                cookbookRepository.FindForChef(chef)
            )
        );
        return retrievalGraph.Retrieve();
    }

    private ListRetrieval<Cookbook> CreateRetrievalGraph(Chef? viewer, ListRetrieval<Cookbook> baseRetriever)
    {
        var filterAccessRights = new FilterAccessRights<Cookbook>(viewer, baseRetriever);
        var orderByProperty = new OrderByProperty<Cookbook, Text>(shoppingList => shoppingList.Title, filterAccessRights);
        return orderByProperty;
    }
}