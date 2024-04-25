using System.Numerics;

namespace Core.ValueObjects;

public record struct Rational<NumberType>(NumberType Numerator, NumberType Denominator) where NumberType : IBinaryInteger<NumberType>
{
    public override string ToString()
    {
        if (NumberType.One == Denominator)
            return $"{Numerator}";

        return $"{Numerator}/{Denominator}";
    }
};