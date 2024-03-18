using Core.Interfaces;

namespace Core.ValueObjects;

public record class Name(string FirstName, string LastName) : IDisplayable
{
    public string display()
    {
        return $"{FirstName} ${LastName}";
    }
}
