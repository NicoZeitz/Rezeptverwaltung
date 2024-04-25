using Core.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        SpoonSize.SERVING => "Servierlöffel",
        _ => throw new NotImplementedException(),
    };

    public static SpoonSize? FromDisplayUnit(string displayUnit) => displayUnit.ToLower() switch
    {
        "teelöffel" => SpoonSize.TEA,
        "dessertlöffel" => SpoonSize.DESSERT,
        "löffel" => SpoonSize.TABLE,
        "servierlöffel" => SpoonSize.SERVING,
        _ => null,
    };
}

public record class Spoon(uint Amount, SpoonSize Size) : MeasurementUnit
{
    public string DisplayUnit => Size.ToDisplayUnit();
    public string DisplayAmount => Amount.ToString();
    public override string ToString() => $"{DisplayAmount} {DisplayUnit}";
    public string display() => ToString();


    public CombinedMeasurementUnit Combine(CombinedMeasurementUnit other, Rational<int> scalar)
    {
        return other with
        {
            Weight = other.Weight + (Size switch
            {
                SpoonSize.TEA => 10,
                SpoonSize.DESSERT => 13,
                SpoonSize.TABLE => 20,
                SpoonSize.SERVING => 25,
                _ => throw new NotImplementedException()
            }) * scalar
        };
    }
}
