using Core.Interfaces;

namespace Core.ValueObjects;

public record struct Name(string FirstName, string LastName) : Displayable, IComparable<Name>
{
    public string Display() => $"{FirstName} ${LastName}";

    public string display()
    {
        return $"{FirstName} ${LastName}";
    }

    public int CompareTo(Name other) => string.Compare(Display(), other.Display());
}
