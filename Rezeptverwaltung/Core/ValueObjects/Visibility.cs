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
}