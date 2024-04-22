using Core.Interfaces;
using Core.ValueObjects.MeasurementUnits;

namespace Core.Services.Serialization;

public class PinchSerializer : MeasurementUnitSerializer<Pinch>
{
    public string[] UnitsToDeserialize => ["Prise", "Prisen"];

    public SerializedMeasurementUnit Serialize(Pinch measurementUnit)
    {
        return new SerializedMeasurementUnit(
            "Prise",
            measurementUnit.Number.ToString()
        );
    }

    public Pinch? Deserialize(SerializedMeasurementUnit serializedMeasurementUnit)
    {
        if (!uint.TryParse(serializedMeasurementUnit.Amount, out var amount))
        {
            return null;
        }

        return new Pinch(amount);
    }

}