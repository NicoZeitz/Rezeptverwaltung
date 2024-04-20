namespace Core.ValueObjects;

public record struct Ingredient(string Name) : IComparable<Ingredient>
{
    public int CompareTo(Ingredient other) => string.Compare(Name, other.Name);

    public override string ToString() => Name;
}