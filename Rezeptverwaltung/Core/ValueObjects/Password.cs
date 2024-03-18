namespace Core.ValueObjects;

public record class Password(string Phrase)
{
    public override string ToString()
    {
        return "**********";
    }
}
