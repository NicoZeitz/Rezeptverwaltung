using Core.Interfaces;
using Core.ValueObjects.MeasurementUnits;

namespace Core.Services.Serialization;

public class MeasurementUnitSerializationManager
{
    private Dictionary<Type, MeasurementUnitSerializer> serializers = [];
    private Dictionary<string, MeasurementUnitSerializer> deserializers = [];

    public MeasurementUnitSerializationManager() : base() { }

    public void RegisterSerializer<T>(MeasurementUnitSerializer<T> measurementUnitSerializer)
    where T : MeasurementUnit
    {
        foreach (var type in measurementUnitSerializer.TypesToSerialize)
        {
            serializers[type] = measurementUnitSerializer;
        }
        foreach (var unit in measurementUnitSerializer.UnitsToDeserialize)
        {
            deserializers[unit.ToLower()] = measurementUnitSerializer;
        }
    }

    public MeasurementUnit? DeserializeFrom(SerializedMeasurementUnit serializedMeasurementUnit)
    {
        if (!deserializers.TryGetValue(serializedMeasurementUnit.Unit.ToLower(), out var deserializer))
        {
            return null;
        }

        return deserializer.Deserialize(serializedMeasurementUnit);
    }

    public SerializedMeasurementUnit SerializeInto(MeasurementUnit measurementUnit)
    {
        if (!serializers.TryGetValue(measurementUnit.GetType(), out var serializer))
        {
            throw new ArgumentException("Unknown measurement unit");
        }

        return serializer.Serialize(measurementUnit);
    }
}
