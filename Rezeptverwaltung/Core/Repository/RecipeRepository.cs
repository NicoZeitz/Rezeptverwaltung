using Core.Entities;
using Core.ValueObjects;

namespace Core.Repository;

public interface RecipeRepository
{
    void Add(Recipe recipe);

    void Update(Recipe recipe);

    void Remove(Recipe recipe);

    Recipe? FindByIdentifier(Identifier identifier);

    IEnumerable<Recipe> FindByTitle(Text title);

    IEnumerable<Recipe> FindAll();

    IEnumerable<Recipe> FindByTag(Tag tag);

    IEnumerable<Recipe> FindForChef(Chef chef);

    IEnumerable<Recipe> FindForCookbook(Cookbook cookbook);

    IEnumerable<Recipe> FindForShoppingList(ShoppingList shoppingList);
}