namespace Core.ValueObjects;

public record struct Tag(string Text) : IComparable<Tag>
{
    public int CompareTo(Tag other) => string.Compare(Text, other.Text);

    public override string ToString() => Text;
}
