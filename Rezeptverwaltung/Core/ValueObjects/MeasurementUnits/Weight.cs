using Core.Interfaces;

namespace Core.ValueObjects.MeasurementUnits;

public enum WeightUnit
{
    G,
    KG
}

public record struct Weight(int Amount) : MeasurementUnit
{
    public const WeightUnit G = WeightUnit.G;
    public const WeightUnit KG = WeightUnit.KG;


    public static Weight FromGram(int gram) => new Weight(gram);

    public static Weight FromKilogram(double kilogram) => new Weight((int)(kilogram * 1000));

    public SerializedMeasurementUnit Serialize()
    {
        return new SerializedMeasurementUnit(
            nameof(Weight),
            Amount.ToString(),
            nameof(WeightUnit.G)
        );
    }

    public static MeasurementUnit Deserialize(SerializedMeasurementUnit serializedMeasurementUnit)
    {
        switch(serializedMeasurementUnit.Unit)
        {
            case nameof(WeightUnit.KG):
                return FromKilogram(double.Parse(serializedMeasurementUnit.Amount));
            case nameof(WeightUnit.G):
            default:
                return FromGram(int.Parse(serializedMeasurementUnit.Amount));

        }
    }
}
