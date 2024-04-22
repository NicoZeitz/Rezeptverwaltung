using Core.Interfaces;

namespace Core.ValueObjects.MeasurementUnits;

public record class Piece(uint Amount) : MeasurementUnit
{
    public string DisplayUnit => "x";
    public string DisplayAmount => Amount.ToString();
    public override string ToString() => $"{DisplayAmount} {DisplayUnit}";
}
