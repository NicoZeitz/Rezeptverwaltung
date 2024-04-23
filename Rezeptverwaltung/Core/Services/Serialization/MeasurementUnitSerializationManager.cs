using Core.Interfaces;
using Core.ValueObjects.MeasurementUnits;

namespace Core.Services.Serialization;

public class MeasurementUnitSerializationManager
{
    private readonly Dictionary<Type, MeasurementUnitSerializer> serializers = [];
    private readonly Dictionary<string, MeasurementUnitSerializer> deserializers = [];

    public MeasurementUnitSerializationManager() : base() { }

    public string[] UnitsToDeserialize => deserializers.Keys.ToArray();

    public Type[] TypesToSerialize => serializers.Keys.ToArray();

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

    public void UnregisterSerializer<T>(MeasurementUnitSerializer<T> measurementUnitSerializer)
    where T : MeasurementUnit
    {
        foreach (var type in measurementUnitSerializer.TypesToSerialize)
        {
            if (serializers.TryGetValue(type, out var serializer) && serializer == measurementUnitSerializer)
            {
                serializers.Remove(type);
            }
        }
        foreach (var unit in measurementUnitSerializer.UnitsToDeserialize)
        {
            if (deserializers.TryGetValue(unit.ToLower(), out var deserializer) && deserializer == measurementUnitSerializer)
            {
                deserializers.Remove(unit.ToLower());
            }
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
