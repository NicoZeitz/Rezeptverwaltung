using Core.ValueObjects;

namespace Core.Entities;

public class ShoppingList
{
    public Identifier Identifier { get; }
    public Identifier Creator { get;  } // TODO: KochIdentifier?
    public List<PortionedRecipe> PortionedRecipes { get; }
}
