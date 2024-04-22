using Core.Data;
using Core.Interfaces;

namespace Core.ValueObjects.MeasurementUnits;

public enum VolumeUnit : uint
{
    ml = 1,
    l = 1000,
    kl = 1000 * 1000,
}

public record struct Volume(uint Amount) : MeasurementUnit
{
    public readonly string DisplayUnit => UnitEnumExtensions<VolumeUnit>.GetNameBelowValue(Amount)!;

    public readonly string DisplayAmount
    {
        get
        {
            var unitMultiplier = (double)UnitEnumExtensions<VolumeUnit>.GetEnumBelowValue(Amount)!;
            var normalizedAmount = Amount / unitMultiplier;
            return normalizedAmount.ToString("0.###");
        }
    }

    public override readonly string ToString() => $"{DisplayAmount} {DisplayUnit}";
}
