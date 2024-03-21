namespace Core.ValueObjects;

public record struct Identifier(Guid Id)
{
    public override string ToString() => Id.ToString();

    public static Identifier NewId() => new Identifier(Guid.NewGuid());

    public static Identifier Parse(string value) => new Identifier(Guid.Parse(value));
}
