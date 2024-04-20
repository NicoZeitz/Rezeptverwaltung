using Core.Interfaces;
using Core.Services;

namespace Core.ValueObjects.MeasurementUnits;

public record class Pinch(int Number) : MeasurementUnit
{
    public static Pinch Deserialize(SerializedMeasurementUnit serializedMeasurementUnit)
    {
        return new Pinch(int.Parse(serializedMeasurementUnit.Amount));
    }

    public static SerializedMeasurementUnit Serialize(Pinch measurementUnit)
    {
        return new SerializedMeasurementUnit(
            nameof(Pinch),
            measurementUnit.Number.ToString(),
            string.Empty
        );
    }

    public string display()
    {
        if (Number == 1)
        {
            return $"{Number} Prise";
        }
        return $"{Number} Prisen";
    }

    public override string ToString() => display();
}
