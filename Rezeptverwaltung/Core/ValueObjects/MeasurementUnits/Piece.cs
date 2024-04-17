using Core.Interfaces;
using Core.Services;

namespace Core.ValueObjects.MeasurementUnits;

public record class Piece(int Amount) : MeasurementUnit
{
    public static Piece Deserialize(SerializedMeasurementUnit serializedMeasurementUnit)
    {
        return new Piece(int.Parse(serializedMeasurementUnit.Amount));
    }

    public static SerializedMeasurementUnit Serialize(Piece measurementUnit)
    {
        return new SerializedMeasurementUnit(
            nameof(Piece),
            measurementUnit.Amount.ToString(),
            string.Empty
        );
    }

    public string display()
    {
        if (Amount == 1)
        {
            return $"{Amount} Stück";
        }
        return $"{Amount} Stücke";
    }
}
