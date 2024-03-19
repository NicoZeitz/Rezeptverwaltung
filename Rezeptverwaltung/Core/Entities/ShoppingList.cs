using Core.ValueObjects;

namespace Core.Entities;

public class ShoppingList
{
    public Identifier Identifier { get; }
    public Identifier Creator { get;  } // TODO: KochIdentifier?
    public List<PortionedRecipe> PortionedRecipes { get; }

    public virtual bool Equals(ShoppingList? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (other is null)
            return false;

        return Identifier == other.Identifier;
    }

    public override int GetHashCode() => Identifier.GetHashCode();

    public override bool Equals(object? obj) => Equals(obj as ShoppingList);

    public static bool operator ==(ShoppingList? left, ShoppingList? right) =>
        ReferenceEquals(left, right) || (left is not null && left.Equals(right));

    public static bool operator !=(ShoppingList? left, ShoppingList? right) => !(left == right);
}
