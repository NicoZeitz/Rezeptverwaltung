using Core.Entities;
using Core.Repository;
using Core.ValueObjects;

namespace Core.Services;

public class CreateRecipeService
{
    private readonly ImageService imageService;
    private readonly RecipeRepository recipeRepository;

    public CreateRecipeService(
        ImageService imageService,
        RecipeRepository recipeRepository)
        : base()
    {
        this.imageService = imageService;
        this.recipeRepository = recipeRepository;
    }


    public Recipe CreateRecipe(
        Text title,
        Text description,
        Chef chef,
        Visibility visibility,
        Portion portion,
        Duration duration,
        Tag[] tags,
        PreparationStep[] preparationSteps,
        WeightedIngredient[] ingredients,
        Image image)
    {
        var recipe = new Recipe(
            Identifier.NewId(),
            chef.Username,
            title,
            description,
            visibility,
            portion,
            duration,
            tags,
            preparationSteps,
            ingredients
        );

        recipeRepository.Add(recipe);
        imageService.SaveImage(recipe, image);
        return recipe;
    }
}