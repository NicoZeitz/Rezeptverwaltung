using Core.ValueObjects;

namespace Core.Interfaces;

public interface MeasurementUnit : Displayable, Combinable<CombinedMeasurementUnit, int>
{
    public string DisplayUnit { get; }
    public string DisplayAmount { get; }
}

public record struct CombinedMeasurementUnit(Rational<int> Count, Rational<int> Weight);