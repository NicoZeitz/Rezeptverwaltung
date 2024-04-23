using Core.Interfaces;
using Core.ValueObjects;

namespace Core.Entities;

public class ShoppingList : IEquatable<ShoppingList>, UniqueIdentity, AccessRights
{
    public Identifier Identifier { get; }
    public Text Title { get; }
    public Visibility Visibility { get; }
    public Username Creator { get; }
    public IList<PortionedRecipe> PortionedRecipes { get; }

    public ShoppingList(Identifier id, Text title, Visibility visibility, Username creator, IEnumerable<PortionedRecipe> portionedRecipes)
    {
        Identifier = id;
        Title = title;
        Visibility = visibility;
        Creator = creator;
        PortionedRecipes = portionedRecipes.ToList();
    }

    public string GetUniqueIdentity() => Identifier.Id.ToString();

    public bool IsVisibleTo(Chef viewer) => viewer.Username == Creator || Visibility.IsPublic();

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
