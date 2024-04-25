using Core.Entities;
using Core.ValueObjects;

namespace Server.ValueObjects.PostData;

public record struct NewShoppingListPostData(
    Text Title,
    Chef CurrentChef,
    Visibility Visibility,
    IEnumerable<PortionedRecipe> PortionedRecipes
);