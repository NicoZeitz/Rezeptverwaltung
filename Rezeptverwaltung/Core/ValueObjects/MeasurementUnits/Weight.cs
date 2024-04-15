using Core.Interfaces;
using Core.Services;

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

    // TODO: Dry Verletzung und Magic Number
    public static Weight FromKilogram(double kilogram) => new Weight((int)(kilogram * 1000));

    public static SerializedMeasurementUnit Serialize(Weight measurementUnit)
    {
        return new SerializedMeasurementUnit(
            nameof(Weight),
            measurementUnit.Amount.ToString(),
            nameof(WeightUnit.G)
        );
    }

    public static Weight Deserialize(SerializedMeasurementUnit serializedMeasurementUnit)
    {
        switch (serializedMeasurementUnit.Unit)
        {
            case nameof(WeightUnit.KG):
                return FromKilogram(double.Parse(serializedMeasurementUnit.Amount));
            case nameof(WeightUnit.G):
            default:
                return FromGram(int.Parse(serializedMeasurementUnit.Amount));
        }
    }

    public string display()
    {
        if (Amount < 1000)
        {
            return $"{Amount} g";
        }
        else
        {
            // TODO: Dry Verletzung und Magic Number
            return $"{Amount / 1000} kg";
        }
    }
}
