using Core.Interfaces;

namespace Core.ValueObjects;

public record class WeightedIngredient(
    MeasurementUnit PreparationQuantity,
    Ingredient Ingredient
);