using Core.ValueObjects;

namespace Core.Entities;

public class Cookbook
{
    public Identifier Identifier { get; }
    public Text Title { get; }
    public Text Description { get; }
    public Username Creator { get; }
    public Visibility Visibility { get; }

    public Cookbook(Identifier id, Text title, Text description, Username creator, Visibility visibility)
    {
        Identifier = id;
        Title = title;
        Description = description;
        Creator = creator;
        Visibility = visibility;
    }

    public virtual bool Equals(Cookbook? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (other is null)
            return false;

        return Identifier == other.Identifier;
    }

    public override int GetHashCode() => Identifier.GetHashCode();

    public override bool Equals(object? obj) => Equals(obj as Cookbook);

    public static bool operator ==(Cookbook? left, Cookbook? right) =>
        ReferenceEquals(left, right) || (left is not null && left.Equals(right));

    public static bool operator !=(Cookbook? left, Cookbook? right) => !(left == right);
}
