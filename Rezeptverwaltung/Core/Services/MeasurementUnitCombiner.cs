using Core.Interfaces;

namespace Core.Services;

public class MeasurementUnitCombiner
{
    public static MeasurementUnit? Combine(IEnumerable<MeasurementUnit> measurementUnits)
    {
        //return measurementUnits
        //        .GroupBy(measurementUnit => measurementUnit.GetType())
        //        .Select(group => group
        //        .Aggregate((first, second) => first.Combine(second)))
        //        .FirstOrDefault();

        return null; // TODO: Implement
    }
}
