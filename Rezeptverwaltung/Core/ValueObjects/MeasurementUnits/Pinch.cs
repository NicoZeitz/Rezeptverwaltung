using Core.Interfaces;

namespace Core.ValueObjects.MeasurementUnits;

public record struct Pinch(uint Number) : MeasurementUnit
{
    public readonly string DisplayUnit => Number == 1 ? "Prise" : "Prisen";
    public readonly string DisplayAmount => Number.ToString();
    public override readonly string ToString() => $"{DisplayAmount} {DisplayUnit}";
}
