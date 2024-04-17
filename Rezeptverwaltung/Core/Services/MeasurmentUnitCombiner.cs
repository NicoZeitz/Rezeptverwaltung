using Core.Interfaces;

namespace Core.Services;

public class MeasurmentUnitCombiner
{
    public static MeasurementUnit? Combine(IEnumerable<MeasurementUnit> measurementUnits)
    {
        return measurementUnits
                .GroupBy(measurementUnit => measurementUnit.GetType())
                .Select(group => group
                .Aggregate((first, second) => first.Combine(second)))
                .FirstOrDefault();
    }
}
