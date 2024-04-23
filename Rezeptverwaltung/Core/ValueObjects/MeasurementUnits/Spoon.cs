using Core.Interfaces;

namespace Core.ValueObjects.MeasurementUnits;

public enum SpoonSize
{
    TEA,
    DESSERT,
    TABLE,
    SERVING,
}

public static class SpoonSizeExtensions
{
    public static string ToDisplayUnit(this SpoonSize spoonSize) => spoonSize switch
    {
        SpoonSize.TEA => "Teelöffel",
        SpoonSize.DESSERT => "Dessertlöffel",
        SpoonSize.TABLE => "Löffel",
        SpoonSize.SERVING => "Schöpfkelle",
        _ => throw new NotImplementedException(),
    };

    public static SpoonSize? FromDisplayUnit(string displayUnit) => displayUnit.ToLower() switch
    {
        "teelöffel" => SpoonSize.TEA,
        "dessertlöffel" => SpoonSize.DESSERT,
        "löffel" => SpoonSize.TABLE,
        "schöpfkelle" => SpoonSize.SERVING,
        _ => null,
    };
}

public record class Spoon(uint Amount, SpoonSize Size) : MeasurementUnit
{
    public string DisplayUnit => Size.ToDisplayUnit();
    public string DisplayAmount => Amount.ToString();
    public override string ToString() => $"{DisplayAmount} {DisplayUnit}";
    public string display() => ToString();
}
