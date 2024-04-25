using System.Numerics;
using core.Services;
using Core.ValueObjects;

namespace Core.Services;

public class AddRationalsService<T> where T : IBinaryInteger<T>
{
    private readonly ReduceFractionService<T> reduceFractionService;

    public AddRationalsService(ReduceFractionService<T> reduceFractionService) : base()
    {
        this.reduceFractionService = reduceFractionService;
    }

    public Rational<T> AddRationals(Rational<T> a, Rational<T> b)
    {
        var newFraction = new Rational<T>(
            a.Numerator * b.Denominator + b.Numerator * a.Denominator,
            a.Denominator * b.Denominator
        );
        return reduceFractionService.ReduceFraction(newFraction);
    }
}