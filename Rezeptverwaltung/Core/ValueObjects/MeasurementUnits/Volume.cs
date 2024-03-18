using Core.Interfaces;

namespace Core.ValueObjects.MeasurementUnits;

public enum VolumeUnit
{
    L,
    ML
}

public record class Volume(int Amount) : MeasurementUnit
{
}
