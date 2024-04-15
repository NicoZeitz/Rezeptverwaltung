using Core.Entities;
using Core.ValueObjects;

namespace Core.Repository;

public interface ShoppingListRepository
{
    void add(ShoppingList shoppingList);

    void remove(ShoppingList shoppingList);

    ShoppingList? findByIdentifier(Identifier identifier);
}
