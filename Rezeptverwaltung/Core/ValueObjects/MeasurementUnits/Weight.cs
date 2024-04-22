using Core.Data;
using Core.Interfaces;

namespace Core.ValueObjects.MeasurementUnits;

public enum WeightUnit : uint
{
    g = 1,
    kg = 1000,
    t = 1000 * 1000,
}

public record class Weight(uint Amount) : MeasurementUnit
{
    public string DisplayUnit => UnitEnumExtensions<WeightUnit>.GetNameBelowValue(Amount)!;

    public string DisplayAmount
    {
        get
        {
            var unitMultiplier = (double)UnitEnumExtensions<WeightUnit>.GetEnumBelowValue(Amount)!;
            var normalizedAmount = Amount / unitMultiplier;
            return normalizedAmount.ToString("0.###");
        }
    }

    public override string ToString() => $"{DisplayAmount} {DisplayUnit}";
}
