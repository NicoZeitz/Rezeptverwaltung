using Core.Data;
using Core.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            var unitMultiplier = (double)(UnitEnumExtensions<VolumeUnit>.GetEnumBelowValue(Amount) ?? VolumeUnit.ml);
            var normalizedAmount = Amount / unitMultiplier;
            return normalizedAmount.ToString("0.###");
        }
    }

    public override string ToString() => $"{DisplayAmount} {DisplayUnit}";
    public string display() => ToString();

    public CombinedMeasurementUnit Combine(CombinedMeasurementUnit other, Rational<int> scalar)
    {
        return other with
        {
            // Diese Umrechnung mag für alles außer Wasser falsch erscheinen,
            // aber wir leben in einer Welt die zu 71% mit Wasser bedeckt ist. (mic drop)
            Weight = other.Weight + (int)Amount * scalar,
        };
    }
}
