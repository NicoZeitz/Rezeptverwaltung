using Core.Entities;
using Core.ValueObjects;

namespace Core.Repository;

public interface ShoppingListRepository
{
    void Add(ShoppingList shoppingList);

    void Update(ShoppingList shoppingList);

    void Remove(ShoppingList shoppingList);

    ShoppingList? FindByIdentifier(Identifier identifier);

    IEnumerable<ShoppingList> FindForChef(Chef chef);
}
