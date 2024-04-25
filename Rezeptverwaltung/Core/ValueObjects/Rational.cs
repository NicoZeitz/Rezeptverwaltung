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

    public static Rational<NumberType> One
        => new Rational<NumberType>(NumberType.One, NumberType.One);

    public static Rational<NumberType> Zero
        => new Rational<NumberType>(NumberType.Zero, NumberType.One);


    public static Rational<NumberType> operator *(Rational<NumberType> rational, NumberType scalar)
    {
        return new Rational<NumberType>(rational.Numerator *  scalar, rational.Denominator);
    }

    public static Rational<NumberType> operator *(NumberType scalar, Rational<NumberType> rational)
        => rational * scalar;

    public static Rational<NumberType> operator +(Rational<NumberType> rational1, Rational<NumberType> rational2)
    {
        return new Rational<NumberType>(
            rational1.Numerator * rational2.Denominator + rational2.Numerator * rational1.Denominator,
            rational1.Denominator * rational2.Denominator
        );
    }

    public static Rational<NumberType> operator -(Rational<NumberType> rational1, Rational<NumberType> rational2)
    {
        return new Rational<NumberType>(
            rational1.Numerator * rational2.Denominator - rational2.Numerator * rational1.Denominator,
            rational1.Denominator * rational2.Denominator
        );
    }

    public static Rational<NumberType> operator *(Rational<NumberType> rational1, Rational<NumberType> rational2)
    {
        return new Rational<NumberType>(
            rational1.Numerator * rational2.Numerator,
            rational1.Denominator * rational2.Denominator
        );
    }

    public static Rational<NumberType> operator /(Rational<NumberType> rational1, Rational<NumberType> rational2)
    {
        return new Rational<NumberType>(
            rational1.Numerator * rational2.Denominator,
            rational1.Denominator * rational2.Numerator
        );
    }
};