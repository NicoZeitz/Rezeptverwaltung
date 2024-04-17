using Core.Interfaces;

namespace Core.ValueObjects.MeasurementUnits;

public record class MeasurementUnitCombination(MeasurementUnit[] MeasurementUnits) : MeasurementUnit
{
    public string display()
    {
        return string.Join(" und ", MeasurementUnits
            .Select(measurementUnit => measurementUnit.display()));
    }
}
