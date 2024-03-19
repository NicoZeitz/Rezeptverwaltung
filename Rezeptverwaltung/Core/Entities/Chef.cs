using Core.ValueObjects;

namespace Core.Entities;

public class Chef : IEquatable<Chef>
{
    public readonly Identifier Identifier;
    public Name Username { get; }
    public Password Password { get; }

    private List<Identifier> CreatedRecipies { get; init; } = new List<Identifier>();
    private List<Identifier> CookBooks { get; init; } = new List<Identifier>();
    private List<Identifier> SavedRecipies { get; init; } = new List<Identifier>();

    public Chef(Identifier identifier, Name username)
    {
        Identifier = identifier;
        Username = username;
    }

    public virtual bool Equals(Chef? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (other is null)
            return false;

        return Identifier == other.Identifier;
    }

    public override int GetHashCode() => Identifier.GetHashCode();

    public override bool Equals(object? obj) => Equals(obj as Chef);

    public static bool operator ==(Chef? left, Chef? right) =>
        ReferenceEquals(left, right) || (left is not null && left.Equals(right));

    public static bool operator !=(Chef? left, Chef? right) => !(left == right);
}
