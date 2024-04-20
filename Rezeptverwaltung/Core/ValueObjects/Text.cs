using Core.Interfaces;

namespace Core.ValueObjects;

public record struct Text(string Value) : IComparable<Text>, Displayable
{
    public int CompareTo(Text other) => string.Compare(Value, other.Value);

    public string display() => Value;

    public override string ToString() => Value.ToString();
}
