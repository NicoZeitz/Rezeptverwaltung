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
        Chef chef,
        Visibility visibility,
        IEnumerable<PortionedRecipe> recipes)
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