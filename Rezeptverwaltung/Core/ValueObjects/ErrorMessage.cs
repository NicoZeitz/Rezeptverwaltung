using Core.Interfaces;

namespace Core.ValueObjects;

public record struct ErrorMessage(string Message) : Displayable, IComparable<ErrorMessage>
{
    public int CompareTo(ErrorMessage other) => string.Compare(Message, other.Message);

    public string display() => Message;

    public override string ToString() => Message;
}
