using Core.Data;
using Core.Interfaces;

namespace Core.ValueObjects.MeasurementUnits;

public enum WeightUnit : uint
{
    g = 1,
    kg = 1000,
    t = 1000 * 1000,
}

public record struct Weight(uint Amount) : MeasurementUnit
{
    public readonly string DisplayUnit => UnitEnumExtensions<WeightUnit>.GetNameBelowValue(Amount)!;

    public readonly string DisplayAmount
    {
        get
        {
            var unitMultiplier = (double)UnitEnumExtensions<WeightUnit>.GetEnumBelowValue(Amount)!;
            var normalizedAmount = Amount / unitMultiplier;
            return normalizedAmount.ToString("0.###");
        }
    }

    public override readonly string ToString() => $"{DisplayAmount} {DisplayUnit}";
}
