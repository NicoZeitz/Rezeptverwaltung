namespace Core.ValueObjects;

public record struct Username(string Name)
{
    public override string ToString() => Name;
}