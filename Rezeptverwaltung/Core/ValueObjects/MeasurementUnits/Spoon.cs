using Core.Interfaces;
using Core.Services;

namespace Core.ValueObjects.MeasurementUnits;

public enum SpoonSize
{
    LARGE,
}

public record class Spoon(int Amount, SpoonSize Size) : MeasurementUnit
{
    public static Spoon Deserialize(SerializedMeasurementUnit serializedMeasurementUnit)
    {
        return new Spoon(int.Parse(serializedMeasurementUnit.Amount), SpoonSize.LARGE);
    }

    public static SerializedMeasurementUnit Serialize(Spoon measurementUnit)
    {
        return new SerializedMeasurementUnit(
            nameof(Spoon),
            measurementUnit.Amount.ToString(),
            measurementUnit.Size switch
            {
                SpoonSize.LARGE => nameof(SpoonSize.LARGE),
            }
        );
    }

    public string display()
    {
        return Size switch
        {
            SpoonSize.LARGE => $"{Amount} Löffel",
        };
    }
}
