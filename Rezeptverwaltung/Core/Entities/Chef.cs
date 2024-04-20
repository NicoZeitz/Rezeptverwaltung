using Core.Interfaces;
using Core.ValueObjects;

namespace Core.Entities;

public class Chef : IEquatable<Chef>, UniqueIdentity
{
    public Username Username { get; }
    public Name Name { get; set; }
    public HashedPassword HashedPassword { get; set; }

    public Chef(Username username, Name name, HashedPassword hashedPassword)
    {
        Username = username;
        Name = name;
        HashedPassword = hashedPassword;
    }

    public string GetUniqueIdentity() => Username.Name;

    public virtual bool Equals(Chef? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (other is null)
            return false;

        return Username == other.Username;
    }

    public override int GetHashCode() => Username.GetHashCode();

    public override bool Equals(object? obj) => Equals(obj as Chef);

    public static bool operator ==(Chef? left, Chef? right) =>
        ReferenceEquals(left, right) || (left is not null && left.Equals(right));

    public static bool operator !=(Chef? left, Chef? right) => !(left == right);
}
