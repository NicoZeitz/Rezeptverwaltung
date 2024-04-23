using Core.Entities;
using Core.Repository;
using Core.ValueObjects;

namespace Core.Services;

public class DeleteShoppingListService
{
    private readonly ShoppingListRepository shoppingListRepository;

    public DeleteShoppingListService(ShoppingListRepository shoppingListRepository)
    {
        this.shoppingListRepository = shoppingListRepository;
    }

    public bool DeleteShoppingList(Identifier id, Chef chef)
    {
        var shoppingList = shoppingListRepository.FindByIdentifier(id);
        if (shoppingList is null)
        {
            return false;
        }

        if (shoppingList.Creator != chef.Username)
        {
            return false;
        }

        shoppingListRepository.Remove(shoppingList);
        return true;
    }
}