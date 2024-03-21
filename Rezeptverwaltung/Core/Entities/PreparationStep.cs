using Core.ValueObjects;

namespace Core.Entities;

public class PreparationStep
{
    public Identifier Identifier { get; }
    public Text Description { get; }

    public PreparationStep(Identifier identifier, Text description)
    {
        Identifier = identifier;
        Description = description;
    }

    public virtual bool Equals(PreparationStep? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (other is null)
            return false;

        return Identifier == other.Identifier;
    }

    public override int GetHashCode() => Identifier.GetHashCode();

    public override bool Equals(object? obj) => Equals(obj as PreparationStep);

    public static bool operator ==(PreparationStep? left, PreparationStep? right) =>
        ReferenceEquals(left, right) || (left is not null && left.Equals(right));

    public static bool operator !=(PreparationStep? left, PreparationStep? right) => !(left == right);
}
