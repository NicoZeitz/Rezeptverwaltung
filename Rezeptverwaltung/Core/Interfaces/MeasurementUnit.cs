using Core.ValueObjects.MeasurementUnits;

namespace Core.Interfaces;

public interface MeasurementUnit : Displayable
{
    public MeasurementUnit Combine(MeasurementUnit other)
    {
        return new MeasurementUnitCombination(new[] { this, other });
    }
}
