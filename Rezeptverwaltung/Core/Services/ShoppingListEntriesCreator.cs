using Core.Entities;

namespace Core.Services;

public class ShoppingListEntriesCreator
{
    public static IEnumerable<object> Create(ShoppingList shoppingList)
    {
        return [];
        //shoppingList.PortionedRecipes
        //return shoppingListEntries
        //        .GroupBy(shoppingListEntry => shoppingListEntry.ProductId)
        //        .Select(group => group
        //                       .Aggregate((first, second) => first.Combine(second)))
        //        .FirstOrDefault();
    }
}
