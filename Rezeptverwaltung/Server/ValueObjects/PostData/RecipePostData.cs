using Core.ValueObjects;
using Server.ContentParser;

namespace Server.ValueObjects.PostData;

public record struct NewRecipePostData(
    Text Title,
    Text Description,
    Portion Portion,
    Duration Duration,
    Visibility Visibility,
    Username Chef,
    Tag[] Tags,
    WeightedIngredient[] Ingredients,
    PreparationStep[] PreparationSteps,
    ContentData Image
);

public record struct EditRecipePostData(
    Identifier RecipeId,
    Text Title,
    Text Description,
    Portion Portion,
    Duration Duration,
    Visibility Visibility,
    Tag[] Tags,
    WeightedIngredient[] Ingredients,
    PreparationStep[] PreparationSteps,
    ContentData Image
);