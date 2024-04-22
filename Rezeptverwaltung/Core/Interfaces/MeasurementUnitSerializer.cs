using Core.ValueObjects.MeasurementUnits;

namespace Core.Interfaces;

public interface MeasurementUnitSerializer<T> where T : MeasurementUnit
{
    public Type[] TypesToSerialize => [typeof(T)];
    public string[] UnitsToDeserialize { get; }

    public SerializedMeasurementUnit Serialize(T measurementUnit);
    public T? Deserialize(SerializedMeasurementUnit serializedMeasurementUnit);
}