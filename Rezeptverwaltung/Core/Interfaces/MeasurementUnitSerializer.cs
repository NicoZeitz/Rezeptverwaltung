using Core.ValueObjects.MeasurementUnits;

namespace Core.Interfaces;

public interface MeasurementUnitSerializer
{
    Type[] TypesToSerialize { get; }
    string[] UnitsToDeserialize { get; }

    public SerializedMeasurementUnit Serialize(MeasurementUnit measurementUnit);
    public MeasurementUnit? Deserialize(SerializedMeasurementUnit serializedMeasurementUnit);
}

public interface MeasurementUnitSerializer<T> : MeasurementUnitSerializer where T : MeasurementUnit
{
    Type[] MeasurementUnitSerializer.TypesToSerialize => [typeof(T)];

    public SerializedMeasurementUnit Serialize(T measurementUnit);
    public new T? Deserialize(SerializedMeasurementUnit serializedMeasurementUnit);

    MeasurementUnit? MeasurementUnitSerializer.Deserialize(SerializedMeasurementUnit serializedMeasurementUnit)
    {
        return Deserialize(serializedMeasurementUnit);
    }

    SerializedMeasurementUnit MeasurementUnitSerializer.Serialize(MeasurementUnit measurementUnit)
    {
        return Serialize((T)measurementUnit);
    }
}