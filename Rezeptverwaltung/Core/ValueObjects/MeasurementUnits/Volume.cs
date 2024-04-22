using Core.Data;
using Core.Interfaces;

namespace Core.ValueObjects.MeasurementUnits;

public enum VolumeUnit : uint
{
    ml = 1,
    l = 1000,
    kl = 1000 * 1000,
}

public record class Volume(uint Amount) : MeasurementUnit
{
    public string DisplayUnit => UnitEnumExtensions<VolumeUnit>.GetNameBelowValue(Amount)!;

    public string DisplayAmount
    {
        get
        {
            var unitMultiplier = (double)UnitEnumExtensions<VolumeUnit>.GetEnumBelowValue(Amount)!;
            var normalizedAmount = Amount / unitMultiplier;
            return normalizedAmount.ToString("0.###");
        }
    }

    public override string ToString() => $"{DisplayAmount} {DisplayUnit}";
}
