using Core.Interfaces;
using Core.ValueObjects.MeasurementUnits;

namespace Core.Services;

public record SerializedMeasurementUnit(string Name, string Unit, string Amount);

public class MeasurementUnitSerializationManager
{
    public MeasurementUnitSerializationManager()
    {
    }

    public MeasurementUnit? DeserializeFrom(SerializedMeasurementUnit serializedMeasurementUnit)
    {
        // TODO: Open-closed principle violation
        switch (serializedMeasurementUnit.Name)
        {
            case nameof(Piece):
                return Piece.Deserialize(serializedMeasurementUnit);
            case nameof(Pinch):
                return Pinch.Deserialize(serializedMeasurementUnit);
            case nameof(Spoon):
                return Spoon.Deserialize(serializedMeasurementUnit);
            case nameof(Volume):
                return Volume.Deserialize(serializedMeasurementUnit);
            case nameof(Weight):
                return Weight.Deserialize(serializedMeasurementUnit);
            default:
                return null;
        }
    }

    public SerializedMeasurementUnit SerializeInto(MeasurementUnit measurementUnit)
    {
        // TODO: Open-closed principle violation
        return measurementUnit switch
        {
            Spoon spoon => Spoon.Serialize(spoon),
            Pinch pinch => Pinch.Serialize(pinch),
            Piece piece => Piece.Serialize(piece),
            Volume volume => Volume.Serialize(volume),
            Weight weight => Weight.Serialize(weight),
            _ => throw new ArgumentException("Unknown measurement unit")
        };
    }
}
