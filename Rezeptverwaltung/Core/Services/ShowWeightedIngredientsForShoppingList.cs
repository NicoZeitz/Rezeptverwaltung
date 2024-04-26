using Core.Entities;
using Core.Interfaces;
using Core.ValueObjects;
using Core.ValueObjects.MeasurementUnits;

namespace Core.Services;

public class ShowWeightedIngredientsForShoppingList
{
    private readonly CeilRationalService<int> ceilRationalService;
    private readonly ShowPortionedRecipesFromShoppingList showPortionedRecipesFromShoppingList;

    public ShowWeightedIngredientsForShoppingList(
        CeilRationalService<int> ceilRationalService,
        ShowPortionedRecipesFromShoppingList showPortionedRecipesFromShoppingList)
        : base()
    {
        this.ceilRationalService = ceilRationalService;
        this.showPortionedRecipesFromShoppingList = showPortionedRecipesFromShoppingList;
    }

    public IEnumerable<WeightedIngredient> ShowIngredients(ShoppingList shoppingList, Chef? viewer)
    {
        var recipesWithPortion = showPortionedRecipesFromShoppingList.ShowRecipes(shoppingList, viewer);
        var uniqueRecipesWithPortion = DeduplicateRecipes(recipesWithPortion);
        var weightedIngredientWithPortion = ScaleIngredientsByRecipePortion(uniqueRecipesWithPortion);
        var weightedIngredients = GroupWeightedIngredientsWithPortionByIngredient(weightedIngredientWithPortion);
        var orderedWeightedIngredients = OrderWeightedIngredients(weightedIngredients);
        var combinedMeasurementUnits = CombineMeasurementUnits(orderedWeightedIngredients);
        return combinedMeasurementUnits;
    }

    private IDictionary<Recipe, Portion> DeduplicateRecipes(IEnumerable<(Portion, Recipe)> recipesWithPortion)
    {
        var uniqueRecipesWithPortion = new Dictionary<Recipe, Portion>();

        foreach (var (portion, recipe) in recipesWithPortion)
        {
            var existingPortion = uniqueRecipesWithPortion.GetValueOrDefault(recipe, new Portion(Rational<int>.Zero));
            uniqueRecipesWithPortion[recipe] = new Portion(existingPortion.Amount + portion.Amount);
        }

        return uniqueRecipesWithPortion;
    }

    private IEnumerable<(Ingredient, MeasurementUnit, Rational<int>)> ScaleIngredientsByRecipePortion(IDictionary<Recipe, Portion> portionedRecipes)
    {
        foreach (var (recipe, portion) in portionedRecipes)
        {
            var recipePersonAmount = portion.Amount / recipe.Portion.Amount;

            foreach (var ingredient in recipe.WeightedIngredients)
            {
                yield return (ingredient.Ingredient, ingredient.PreparationQuantity, recipePersonAmount);
            }
        }
    }

    private IDictionary<Ingredient, List<PortionedMeasurementUnit>> GroupWeightedIngredientsWithPortionByIngredient(IEnumerable<(Ingredient, MeasurementUnit, Rational<int>)> weightedIngredientsWithPortion)
    {
        var weightedIngredients = new Dictionary<Ingredient, List<PortionedMeasurementUnit>>();

        foreach (var (ingredient, preparationQuantity, recipePersonAmount) in weightedIngredientsWithPortion)
        {
            var portionedMeasurementUnit = new PortionedMeasurementUnit(recipePersonAmount, preparationQuantity);
            var existingPortionedMeasurementUnits = weightedIngredients.GetValueOrDefault(ingredient, []);
            existingPortionedMeasurementUnits.Add(portionedMeasurementUnit);
            weightedIngredients[ingredient] = existingPortionedMeasurementUnits;
        }

        return weightedIngredients;
    }

    private IDictionary<Ingredient, List<PortionedMeasurementUnit>> OrderWeightedIngredients(IDictionary<Ingredient, List<PortionedMeasurementUnit>> weightedIngredients)
    {
        return weightedIngredients
            .OrderBy(weightedIngredient => weightedIngredient.Key)
            .ToDictionary();
    }

    private IEnumerable<WeightedIngredient> CombineMeasurementUnits(IDictionary<Ingredient, List<PortionedMeasurementUnit>> weightedIngredients)
    {
        foreach (var (ingredient, portionedMeasurementUnits) in weightedIngredients)
        {
            var combinedMeasurementUnit = CombineSingleMeasurementUnit(portionedMeasurementUnits);

            if (combinedMeasurementUnit.Count != Rational<int>.Zero)
            {
                yield return CreateNewWeightedPieceFrom(ingredient, combinedMeasurementUnit);
            }

            if (combinedMeasurementUnit.Weight != Rational<int>.Zero)
            {
                yield return CreateNewWeightedWeightFrom(ingredient, combinedMeasurementUnit);
            }
        }
    }

    private CombinedMeasurementUnit CombineSingleMeasurementUnit(IEnumerable<PortionedMeasurementUnit> portionedMeasurementUnits)
    {
        var neutralCombinedMeasurementUnit = new CombinedMeasurementUnit(Rational<int>.Zero, Rational<int>.Zero);

        var combinedMeasurementUnit = neutralCombinedMeasurementUnit;

        foreach (var portionedMeasurementUnit in portionedMeasurementUnits)
        {
            combinedMeasurementUnit = portionedMeasurementUnit.MeasurementUnit.Combine(combinedMeasurementUnit, portionedMeasurementUnit.Portion);
        }

        return combinedMeasurementUnit;
    }

    private WeightedIngredient CreateNewWeightedPieceFrom(Ingredient ingredient, CombinedMeasurementUnit combinedMeasurementUnit)
    {
        var piece = ceilRationalService.CeilRational(combinedMeasurementUnit.Count);
        return new WeightedIngredient(
            new Piece((uint)piece),
            ingredient
        );
    }

    private WeightedIngredient CreateNewWeightedWeightFrom(Ingredient ingredient, CombinedMeasurementUnit combinedMeasurementUnit)
    {
        var weight = ceilRationalService.CeilRational(combinedMeasurementUnit.Weight);
        return new WeightedIngredient(
            new Weight((uint)weight),
            ingredient
        );
    }

    private record struct PortionedMeasurementUnit(Rational<int> Portion, MeasurementUnit MeasurementUnit);
}