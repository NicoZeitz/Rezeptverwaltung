using Core.Entities;
using Core.Repository;
using Core.ValueObjects;

namespace Core.Services;

public class CreateShoppingListService
{
    private readonly ShoppingListRepository shoppingListRepository;

    public CreateShoppingListService(ShoppingListRepository shoppingListRepository)
        : base()
    {
        this.shoppingListRepository = shoppingListRepository;
    }

    public ShoppingList CreateShoppingList(
        Text title,
        Visibility visibility,
        Chef chef,
        IEnumerable<PortionedRecipe> recipes) // TODO: better ux
    {
        var shoppingList = new ShoppingList(
            Identifier.NewId(),
            title,
            visibility,
            chef.Username,
            recipes
        );

        shoppingListRepository.Add(shoppingList);
        return shoppingList;
    }
}