using Core.ValueObjects;
using Server.ContentParser;

namespace Server.RequestHandler;

public record struct RecipePostData(
    Text Title,
    Text Description,
    Portion Portion,
    Duration Duration,
    Username Chef,
    Tag[] Tags,
    WeightedIngredient[] Ingredients,
    PreparationStep[] PreparationSteps,
    ContentData Image
);