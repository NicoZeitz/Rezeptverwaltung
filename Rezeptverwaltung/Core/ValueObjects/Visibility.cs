namespace Core.ValueObjects;

public enum Visibility
{
    PRIVATE = 0,
    PUBLIC = 1
}

public static class VisibilityExtensions
{
    public static bool IsPrivate(this Visibility visibility) => visibility == Visibility.PRIVATE;
    public static bool IsPublic(this Visibility visibility) => visibility == Visibility.PUBLIC;

    public static Visibility FromString(string visibility)
    {
        return visibility.ToUpper() switch
        {
            "0" or "PRIVATE" => Visibility.PRIVATE,
            "1" or "PUBLIC" => Visibility.PUBLIC,
            _ => throw new ArgumentException($"Unknown visibility: {visibility}")
        };
    }
}