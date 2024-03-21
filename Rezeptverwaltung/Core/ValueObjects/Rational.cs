using System.Numerics;

namespace Core.ValueObjects;

public record struct Rational<T>(T Numerator, T Denominator) where T : IBinaryInteger<T>;