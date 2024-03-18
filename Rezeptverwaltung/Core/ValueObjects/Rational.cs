using System.Numerics;

namespace Core.ValueObjects;

public record class Rational<T>(T Denominator, T Numerator) where T : IBinaryInteger<T>
{
}
