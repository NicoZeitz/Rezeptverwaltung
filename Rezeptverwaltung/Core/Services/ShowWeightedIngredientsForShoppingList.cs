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

        var uniqueRecipesWithPortion = new Dictionary<Recipe, Portion>();

        foreach (var (portion, recipe) in recipesWithPortion)
        {
            if (uniqueRecipesWithPortion.TryGetValue(recipe, out var existingPortion))
            {
                uniqueRecipesWithPortion[recipe] = new Portion(existingPortion.Amount + portion.Amount);
            }
            else
            {
                uniqueRecipesWithPortion[recipe] = portion;
            }
        }

        var weightedIngredientWithPortion = new List<(Ingredient, MeasurementUnit, Rational<int>)>();

        foreach (var (recipe, portion) in uniqueRecipesWithPortion)
        {
            var recipePersonAmount = portion.Amount / recipe.Portion.Amount;

            foreach (var ingredient in recipe.WeightedIngredients)
            {
                weightedIngredientWithPortion.Add((ingredient.Ingredient, ingredient.PreparationQuantity, recipePersonAmount));
            }
        }

        var weightedIngredients = new Dictionary<Ingredient, List<PortionedMeasurementUnits>>();

        foreach (var (ingredient, preparationQuantity, recipePersonAmount) in weightedIngredientWithPortion)
        {
            var portionedMeasurementUnit = new PortionedMeasurementUnits(recipePersonAmount, preparationQuantity);

            if (weightedIngredients.TryGetValue(ingredient, out var existingQuantities))
            {
                existingQuantities.Add(portionedMeasurementUnit);
            }
            else
            {
                weightedIngredients[ingredient] = [portionedMeasurementUnit];
            }
        }

        var neutralCombinedMeasurementUnit = new CombinedMeasurementUnit(Rational<int>.Zero, Rational<int>.Zero);

        foreach (var (ingredient, portionedMeasurementUnits) in weightedIngredients.OrderBy(weightedIngredient => weightedIngredient.Key))
        {
            var combinedMeasurementUnit = neutralCombinedMeasurementUnit;

            foreach (var portionedMeasurementUnit in portionedMeasurementUnits)
            {
                combinedMeasurementUnit = portionedMeasurementUnit.MeasurementUnit.Combine(combinedMeasurementUnit, portionedMeasurementUnit.Portion);
            }

            if (combinedMeasurementUnit.Count != Rational<int>.Zero)
            {
                var pieces = ceilRationalService.CeilRational(combinedMeasurementUnit.Count);
                yield return new WeightedIngredient(
                    new Piece((uint)pieces),
                    ingredient
                );
            }

            if (combinedMeasurementUnit.Weight != Rational<int>.Zero)
            {
                var weight = ceilRationalService.CeilRational(combinedMeasurementUnit.Weight);
                yield return new WeightedIngredient(
                    new Weight((uint)weight),
                    ingredient
                );
            }
        }
    }

    private record struct PortionedMeasurementUnits(Rational<int> Portion, MeasurementUnit MeasurementUnit);
}