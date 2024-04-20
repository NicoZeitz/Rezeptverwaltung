namespace Core.ValueObjects;

public record struct Username(string Name) : IComparable<Username>
{
    public override string ToString() => Name;

    public int CompareTo(Username other) => string.Compare(Name, other.Name);
}