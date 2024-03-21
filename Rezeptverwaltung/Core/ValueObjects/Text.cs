namespace Core.ValueObjects;

public record struct Text(string Value)
{
    public override string ToString() => Value.ToString();
}
