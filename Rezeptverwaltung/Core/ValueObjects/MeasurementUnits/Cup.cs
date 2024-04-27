using Core.Interfaces;

namespace Core.ValueObjects.MeasurementUnits;

public record class Cup(uint Number) : MeasurementUnit
{
    public string DisplayUnit => Number == 1 ? "Tasse" : "Tassen";
    public string DisplayAmount => Number.ToString();
    public override string ToString() => $"{DisplayAmount} {DisplayUnit}";
    public string display() => ToString();

    public CombinedMeasurementUnit Combine(CombinedMeasurementUnit other, Rational<int> scalar)
    {
        return other with
        {
            // Böse magische Zahl zur Umrechnung!
            Weight = other.Weight + 200 * scalar
        };
    }
}
