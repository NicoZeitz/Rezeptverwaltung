using Core.Entities;
using Core.ValueObjects;

namespace Core.Services;

public class ShowWeightedIngredientsForShoppingList
{
    public ShowWeightedIngredientsForShoppingList()
    {
    }

    public IEnumerable<WeightedIngredient> ShowIngredients(ShoppingList shoppingList, Chef? viewer)
    {
        return []; // TODO:
    }
}