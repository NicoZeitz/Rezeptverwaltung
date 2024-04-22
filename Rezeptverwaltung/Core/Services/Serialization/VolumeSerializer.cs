using Core.Data;
using Core.Interfaces;
using Core.ValueObjects.MeasurementUnits;

namespace Core.Services.Serialization;

public class VolumeSerializer : MeasurementUnitSerializer<Volume>
{
    public string[] UnitsToDeserialize => UnitEnumExtensions<VolumeUnit>.Names;

    public SerializedMeasurementUnit Serialize(Volume measurementUnit)
    {
        return new SerializedMeasurementUnit(
            nameof(VolumeUnit.ml),
            measurementUnit.Amount.ToString()
        );
    }

    public Volume? Deserialize(SerializedMeasurementUnit serializedMeasurementUnit)
    {
        if (!double.TryParse(serializedMeasurementUnit.Amount, out var amount))
        {
            return null;
        }

        var unitMultiplier = UnitEnumExtensions<VolumeUnit>.GetValueForName(serializedMeasurementUnit.Unit);

        if (unitMultiplier is null)
        {
            return null;
        }

        var finalAmount = (uint)(amount * unitMultiplier.Value);
        if (finalAmount == 0)
        {
            return null;
        }

        return new Volume(finalAmount);
    }
}