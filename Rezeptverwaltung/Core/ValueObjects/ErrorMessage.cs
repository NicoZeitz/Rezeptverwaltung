namespace Core.ValueObjects;

public record struct ErrorMessage(string Message)
{
    public override string ToString() => Message;
}
