using Core.Interfaces;

namespace Core.ValueObjects.MeasurementUnits;

public record struct Cup(uint Number) : MeasurementUnit
{
    public readonly string DisplayUnit => Number == 1 ? "Tasse" : "Tassen";
    public readonly string DisplayAmount => Number.ToString();
    public override readonly string ToString() => $"{DisplayAmount} {DisplayUnit}";
}
