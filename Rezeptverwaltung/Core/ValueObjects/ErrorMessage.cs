using Core.Interfaces;

namespace Core.ValueObjects;

public record struct ErrorMessage(string Message) : Displayable
{
    public string display() => Message;

    public override string ToString() => Message;
}
