using Core.Interfaces;

namespace Core.ValueObjects.MeasurementUnits;

public enum VolumeUnit
{
    L,
    ML
}

public record class Volume(int Amount) : MeasurementUnit
{
    public static MeasurementUnit Deserialize(string unit, string amount)
    {
        throw new NotImplementedException();
    }

    public (string, string) Serialize()
    {
        throw new NotImplementedException();
    }
}
