namespace Core.Data;

public static class UnitEnumExtensions<E> where E : struct, Enum
{
    public static E[] Enums = (E[])Enum.GetValues(typeof(E));
    public static string[] Names = Enum.GetNames<E>();
    public static uint[] Values = (uint[])Enum.GetValuesAsUnderlyingType<E>();
    public static (string, uint)[] NameValuePairs = Names
        .Zip(Values, (name, value) => (name, value))
        .OrderByDescending(pair => pair.value)
        .ToArray();

    public static E? GetEnumForName(string name)
    {
        foreach (var enumName in Names)
        {
            if (enumName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return Enum.Parse<E>(enumName);
            }
        }
        return null;
    }

    public static uint? GetValueForName(string name)
    {
        foreach (var (enumName, value) in NameValuePairs)
        {
            if (enumName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return value;
            }
        }
        return null;
    }

    public static string? GetNameBelowValue(uint value)
    {
        foreach (var (enumName, enumValue) in NameValuePairs)
        {
            if (value >= enumValue)
            {
                return enumName.ToLower();
            }
        }
        return null;
    }

    public static string? GetNameAboveValue(uint value)
    {
        foreach (var (enumName, enumValue) in NameValuePairs.Reverse())
        {
            if (value <= enumValue)
            {
                return enumName;
            }
        }
        return null;
    }

    public static E? GetEnumBelowValue(uint value)
    {
        var name = GetNameBelowValue(value);
        if (name is null) return null;

        return Enum.Parse<E>(name);
    }

    public static E? GetEnumAboveValue(uint value)
    {
        var name = GetNameAboveValue(value);
        if (name is null) return null;

        return Enum.Parse<E>(name);
    }
}