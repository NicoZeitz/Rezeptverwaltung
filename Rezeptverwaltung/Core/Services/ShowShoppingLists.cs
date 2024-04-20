using Core.Entities;
using Core.Repository;
using Core.Services.Retrieval;
using Core.ValueObjects;

namespace Core.Services;

public class ShowShoppingLists
{
    private readonly ShoppingListRepository shoppingListRepository;

    public ShowShoppingLists(ShoppingListRepository shoppingListRepository) : base()
    {
        this.shoppingListRepository = shoppingListRepository;
    }

    public ShoppingList? ShowSingleShoppingList(Identifier identifier, Chef? viewer)
    {
        var retrievalGraph = CreateRetrievalGraph(
            viewer,
            new OptionalSingleItemListRetrieval<ShoppingList>(
                shoppingListRepository.FindByIdentifier(identifier)
            )
        );
        return retrievalGraph
            .Retrieve()
            .FirstOrDefault();
    }

    public IEnumerable<ShoppingList> ShowShoppingListsForChef(Chef chef, Chef? viewer)
    {
        var retrievalGraph = CreateRetrievalGraph(
            viewer,
            new SimpleListRetrieval<ShoppingList>(shoppingListRepository.FindForChef(chef))
        );
        return retrievalGraph.Retrieve();
    }

    private ListRetrieval<ShoppingList> CreateRetrievalGraph(Chef? viewer, ListRetrieval<ShoppingList> baseRetriever)
    {
        var filterAccessRights = new FilterAccessRights<ShoppingList>(viewer, baseRetriever);
        var orderByProperty = new OrderByProperty<ShoppingList, Text>(shoppingList => shoppingList.Title, filterAccessRights);
        return orderByProperty;
    }
}