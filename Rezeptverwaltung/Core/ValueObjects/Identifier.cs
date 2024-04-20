namespace Core.ValueObjects;

public record struct Identifier(Guid Id) : IComparable<Identifier>
{
    public override string ToString() => Id.ToString();

    public static Identifier NewId() => new Identifier(Guid.NewGuid());

    public static Identifier Parse(string value) => new Identifier(Guid.Parse(value));

    public int CompareTo(Identifier other) => Id.CompareTo(other.Id);
}
