using Core.Interfaces;
using Core.Services;

namespace Core.ValueObjects.MeasurementUnits;

public enum SpoonSize
{
    TEA,
    DESSERT,
    TABLE,
    SERVING,
}

public record class Spoon(int Amount, SpoonSize Size) : MeasurementUnit
{
    public static Spoon Deserialize(SerializedMeasurementUnit serializedMeasurementUnit)
    {
        return new Spoon(int.Parse(serializedMeasurementUnit.Amount), SpoonSize.TABLE);
    }

    public static SerializedMeasurementUnit Serialize(Spoon measurementUnit)
    {
        return new SerializedMeasurementUnit(
            nameof(Spoon),
            measurementUnit.Amount.ToString(),
            measurementUnit.Size switch
            {
                SpoonSize.TEA => nameof(SpoonSize.TEA),
                SpoonSize.DESSERT => nameof(SpoonSize.DESSERT),
                SpoonSize.TABLE => nameof(SpoonSize.TABLE),
                SpoonSize.SERVING => nameof(SpoonSize.SERVING),
            }
        );
    }

    public string display()
    {
        return Size switch
        {
            SpoonSize.TEA => $"{Amount} Teelöffel",
            SpoonSize.DESSERT => $"{Amount} Dessertlöffel",
            SpoonSize.TABLE => $"{Amount} Löffel",
            SpoonSize.SERVING => $"{Amount} Schöpfkellen",
        };
    }

    public override string ToString() => display();
}
