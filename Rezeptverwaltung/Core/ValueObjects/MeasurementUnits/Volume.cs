using Core.Interfaces;
using Core.Services;

namespace Core.ValueObjects.MeasurementUnits;

public enum VolumeUnit
{
    L,
    ML
}

public record class Volume(int Amount) : MeasurementUnit
{
    public static Volume Deserialize(SerializedMeasurementUnit serializedMeasurementUnit)
    {
        switch (serializedMeasurementUnit.Unit)
        {
            case nameof(VolumeUnit.L):
                return new Volume(int.Parse(serializedMeasurementUnit.Amount));
            case nameof(VolumeUnit.ML):
            default:
                // TODO: Dry Verletzung und Magic Number
                return new Volume(int.Parse(serializedMeasurementUnit.Amount) / 1000);
        }
    }

    public static SerializedMeasurementUnit Serialize(Volume measurementUnit)
    {
        return new SerializedMeasurementUnit(
            nameof(Volume),
            measurementUnit.Amount.ToString(),
            nameof(VolumeUnit.ML)
        );
    }

    public string display()
    {
        // TODO: Dry Verletzung und Magic Number
        if (Amount < 1000)
        {
            return $"{Amount} ml";
        }
        else
        {
            // TODO: Dry Verletzung und Magic Number
            return $"{Amount / 1000} l";
        }
    }
}
