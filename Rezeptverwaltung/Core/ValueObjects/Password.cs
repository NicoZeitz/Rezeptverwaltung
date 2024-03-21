namespace Core.ValueObjects;

public record struct Password(string Phrase)
{
    public override string ToString()
    {
        return "**********";
    }
}
