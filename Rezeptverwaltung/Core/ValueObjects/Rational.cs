using System.Numerics;

namespace Core.ValueObjects;

public record struct Rational<NumerType>(NumerType Numerator, NumerType Denominator) where NumerType : IBinaryInteger<NumerType>
{
    public override string ToString()
    {
        if (NumerType.One == Denominator)
            return $"{Numerator}";

        return $"{Numerator}/{Denominator}";
    }
};