namespace Core.ValueObjects;

public record struct Tag(string Text)
{
    public override string ToString() => Text;
}
