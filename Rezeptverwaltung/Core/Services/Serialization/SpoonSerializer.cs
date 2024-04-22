using Core.Data;
using Core.Interfaces;
using Core.ValueObjects.MeasurementUnits;

namespace Core.Services.Serialization;

public class SpoonSerializer : MeasurementUnitSerializer<Spoon>
{
    public string[] UnitsToDeserialize => UnitEnumExtensions<SpoonSize>
        .Enums
        .Select(SpoonSizeExtensions.ToDisplayUnit)
        .ToArray();

    public SerializedMeasurementUnit Serialize(Spoon measurementUnit)
    {
        return new SerializedMeasurementUnit(
            measurementUnit.Size.ToDisplayUnit(),
            measurementUnit.Amount.ToString()
        );
    }

    public Spoon? Deserialize(SerializedMeasurementUnit serializedMeasurementUnit)
    {
        if (!uint.TryParse(serializedMeasurementUnit.Amount, out var amount))
        {
            return null;
        }

        var unit = SpoonSizeExtensions.FromDisplayUnit(serializedMeasurementUnit.Unit);
        if (unit is null)
        {
            return null;
        }

        return new Spoon(amount, unit.Value);
    }
}