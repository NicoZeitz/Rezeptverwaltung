using Core.ValueObjects;
using System.Numerics;

namespace Core.Interfaces;

public interface Combinable<CombinationType, NumberType>
    where NumberType : IBinaryInteger<NumberType>
{
    CombinationType Combine(CombinationType other, Rational<NumberType> scalar);
}