using Core.Data;
using Core.Interfaces;
using Core.ValueObjects.MeasurementUnits;

namespace Core.Services.Serialization;

public class WeightSerializer : MeasurementUnitSerializer<Weight>
{
    public string[] UnitsToDeserialize => UnitEnumExtensions<WeightUnit>.Names;

    public SerializedMeasurementUnit Serialize(Weight measurementUnit)
    {
        return new SerializedMeasurementUnit(
            nameof(WeightUnit.g),
            measurementUnit.Amount.ToString()
        );
    }

    public Weight? Deserialize(SerializedMeasurementUnit serializedMeasurementUnit)
    {
        if (!double.TryParse(serializedMeasurementUnit.Amount, out var amount))
        {
            return null;
        }

        var unitMultiplier = UnitEnumExtensions<WeightUnit>.GetValueForName(serializedMeasurementUnit.Unit);

        if (unitMultiplier is null)
        {
            return null;
        }

        var finalAmount = (uint)(amount * unitMultiplier.Value);
        if (finalAmount == 0)
        {
            return null;
        }

        return new Weight(finalAmount);
    }
}