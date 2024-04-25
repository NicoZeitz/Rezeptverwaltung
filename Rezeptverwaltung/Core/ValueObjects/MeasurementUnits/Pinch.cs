using Core.Interfaces;

namespace Core.ValueObjects.MeasurementUnits;

public record class Pinch(uint Number) : MeasurementUnit
{
    public string DisplayUnit => Number == 1 ? "Prise" : "Prisen";
    public string DisplayAmount => Number.ToString();
    public override string ToString() => $"{DisplayAmount} {DisplayUnit}";
    public string display() => ToString();

    public CombinedMeasurementUnit Combine(CombinedMeasurementUnit other, Rational<int> scalar)
    {
        return other with
        {
            Weight = other.Weight + (int)Number * scalar
        };
    }
}
