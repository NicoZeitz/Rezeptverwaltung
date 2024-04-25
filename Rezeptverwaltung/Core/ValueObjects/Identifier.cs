namespace Core.ValueObjects;

public record struct Identifier(Guid Id) : IComparable<Identifier>
{
    public override readonly string ToString() => Id.ToString();

    public static Identifier NewId() => new Identifier(Guid.NewGuid());

    public static Identifier? Parse(string value)
    {
        if (Guid.TryParse(value, out var id))
        {
            return new Identifier(id);
        }
        return null;
    }

    public readonly int CompareTo(Identifier other) => Id.CompareTo(other.Id);
}
