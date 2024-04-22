using Core.ValueObjects;

namespace Server.ValueObjects.PostData;

public record struct NewShoppingListPostData(
    Text Title,
    Visibility Visibility,
    IEnumerable<PortionedRecipe> PortionedRecipes
);

public record struct EditShoppingListPostData(
    Identifier Identifier,
    Text Title,
    Visibility Visibility,
    IEnumerable<PortionedRecipe> PortionedRecipes
);
