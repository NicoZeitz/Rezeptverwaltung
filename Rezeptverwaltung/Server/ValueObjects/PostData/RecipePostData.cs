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
    IEnumerable<Tag> Tags,
    IEnumerable<WeightedIngredient> Ingredients,
    IList<PreparationStep> PreparationSteps,
    ContentData Image
);

public record struct EditRecipePostData(
    Identifier RecipeId,
    Text Title,
    Text Description,
    Portion Portion,
    Duration Duration,
    Visibility Visibility,
    IEnumerable<Tag> Tags,
    IEnumerable<WeightedIngredient> Ingredients,
    IList<PreparationStep> PreparationSteps,
    ContentData Image
);