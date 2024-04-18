﻿namespace Core.ValueObjects;

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
            "0" => Visibility.PRIVATE,
            "1" => Visibility.PUBLIC,
            _ => throw new ArgumentException($"Unknown visibility: {visibility}")
        };
    }    
}