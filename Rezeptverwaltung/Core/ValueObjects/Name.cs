using Core.Interfaces;

namespace Core.ValueObjects;

public record struct Name(string FirstName, string LastName) : Displayable
{
    public string display()
    {
        return $"{FirstName} ${LastName}";
    }
}
