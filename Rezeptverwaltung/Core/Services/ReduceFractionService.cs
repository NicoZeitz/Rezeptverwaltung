using Core.ValueObjects;
using System.Numerics;

namespace core.Services;

public class ReduceFractionService<T> where T : IBinaryInteger<T>
{
    public Rational<T> ReduceFraction(Rational<T> rational)
    {
        var numerator = rational.Numerator;
        var denominator = rational.Denominator;

        var gcd = GreatestCommonDivisor(numerator, denominator);
        return new Rational<T>(numerator / gcd, denominator / gcd);
    }

    private T GreatestCommonDivisor(T a, T b)
    {
        while (b != T.Zero)
        {
            var t = b;
            b = a % b;
            a = t;
        }
        return a;
    }
}