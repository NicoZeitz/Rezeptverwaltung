using Core.Interfaces;
using Core.ValueObjects;

namespace Core.Entities;

public class WeightedIngredient : IEquatable<WeightedIngredient>
{
    public Identifier Identifier { get; }
    public MeasurementUnit PreparationQuantity { get; }
    public Text Ingredient {  get; }

    public WeightedIngredient(Identifier identifier, MeasurementUnit preparationQuantity, Text ingredient)
    {
        Identifier = identifier;
        PreparationQuantity = preparationQuantity;
        Ingredient = ingredient;
    }

    public virtual bool Equals(WeightedIngredient? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (other is null)
            return false;

        return Identifier == other.Identifier;
    }

    public override int GetHashCode() => Identifier.GetHashCode();

    public override bool Equals(object? obj) => Equals(obj as WeightedIngredient);

    public static bool operator ==(WeightedIngredient? left, WeightedIngredient? right) =>
        ReferenceEquals(left, right) || (left is not null && left.Equals(right));

    public static bool operator !=(WeightedIngredient? left, WeightedIngredient? right) => !(left == right);
}
