namespace Core.Interfaces;

public record SerializedMeasurementUnit(string Name, string Unit, string Amount);

public interface MeasurementUnit
{

    SerializedMeasurementUnit Serialize();
    abstract static MeasurementUnit Deserialize(SerializedMeasurementUnit serializedMeasurementUnit);
}
