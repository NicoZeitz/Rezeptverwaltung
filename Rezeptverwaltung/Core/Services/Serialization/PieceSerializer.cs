using Core.Interfaces;
using Core.ValueObjects.MeasurementUnits;

namespace Core.Services.Serialization;

public class PieceSerializer : MeasurementUnitSerializer<Piece>
{
    public string[] UnitsToDeserialize => ["x"];

    public SerializedMeasurementUnit Serialize(Piece measurementUnit)
    {
        return new SerializedMeasurementUnit(
            "x",
            measurementUnit.Amount.ToString()
        );
    }

    public Piece? Deserialize(SerializedMeasurementUnit serializedMeasurementUnit)
    {
        if (!uint.TryParse(serializedMeasurementUnit.Amount, out var amount))
        {
            return null;
        }

        return new Piece(amount);
    }
}