using Core.Interfaces;
using Core.ValueObjects.MeasurementUnits;

namespace Core.Services.Serialization;

public class CupSerializer : MeasurementUnitSerializer<Cup>
{
    public string[] UnitsToDeserialize => ["Tasse", "Tassen"];

    public SerializedMeasurementUnit Serialize(Cup measurementUnit)
    {
        return new SerializedMeasurementUnit(
            "Tasse",
            measurementUnit.Number.ToString()
        );
    }

    public Cup? Deserialize(SerializedMeasurementUnit serializedMeasurementUnit)
    {
        if (!uint.TryParse(serializedMeasurementUnit.Amount, out var amount))
        {
            return null;
        }

        return new Cup(amount);
    }

}