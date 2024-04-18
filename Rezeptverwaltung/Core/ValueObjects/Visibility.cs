namespace Core.ValueObjects;

public enum Visibility
{
    PRIVATE,
    PUBLIC
}

public static class VisibilityExtensions
{
    public static bool IsPrivate(this Visibility visibility) => visibility == Visibility.PRIVATE;
    public static bool IsPublic(this Visibility visibility) => visibility == Visibility.PUBLIC;

    public static Visibility FromString(string visibility)
    {
        return visibility switch
        {
            nameof(Visibility.PRIVATE) => Visibility.PRIVATE,
            nameof(Visibility.PUBLIC) => Visibility.PUBLIC,
            _ => throw new ArgumentException($"Unknown visibility: {visibility}")
        };
    }    
}