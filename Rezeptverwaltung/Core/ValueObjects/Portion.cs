namespace Core.ValueObjects;

public record struct Portion(Rational<int> Amount)
{
    public override string ToString() => Amount.ToString();
}
