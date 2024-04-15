using Core.Entities;
using Core.ValueObjects;

namespace Core.Repository;

public interface RecipeRepository
{
    void add(Recipe recipe);

    void remove(Recipe recipe);

    Recipe? findByIdentifier(Identifier identifier);

    IEnumerable<Recipe> findByTitle(Text title);

    IEnumerable<Recipe> findAll();

    IEnumerable<Recipe> findForChef(Chef chef);

    IEnumerable<Recipe> findForCookbook(Cookbook cookbook);

    IEnumerable<Recipe> findForShoppingList(ShoppingList shoppingList);
}