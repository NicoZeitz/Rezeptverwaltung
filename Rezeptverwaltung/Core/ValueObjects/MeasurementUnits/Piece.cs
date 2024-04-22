using Core.Interfaces;

namespace Core.ValueObjects.MeasurementUnits;

public record struct Piece(uint Amount) : MeasurementUnit
{
    public readonly string DisplayUnit => "x";
    public readonly string DisplayAmount => Amount.ToString();
    public override readonly string ToString() => $"{DisplayAmount} {DisplayUnit}";
}
