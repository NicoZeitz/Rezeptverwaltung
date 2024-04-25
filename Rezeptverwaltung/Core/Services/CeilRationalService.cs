using Core.ValueObjects;
using System.Numerics;

namespace Core.Services;

public class CeilRationalService<NumberType>
    where NumberType : IBinaryInteger<NumberType>
{
    public NumberType CeilRational(Rational<NumberType> rational)
    {
        var hasRemainder = rational.Numerator % rational.Denominator != NumberType.Zero;
        var flooredValue = (rational.Numerator / rational.Denominator);

        if(hasRemainder)
        {
            return flooredValue + NumberType.One;
        }
        else
        {
            return flooredValue;
        }
    }
}
