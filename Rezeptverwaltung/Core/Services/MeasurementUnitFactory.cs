using Core.Interfaces;
using Core.ValueObjects.MeasurementUnits;

namespace Core.Services;

public class MeasurementUnitFactory
{
    private readonly IDictionary<string, Type> measurementUnits = new Dictionary<string, Type>();
    public MeasurementUnitFactory()
    {
        registerMeasurementUnit(nameof(Weight), typeof(Volume));
        registerMeasurementUnit(nameof(Volume), typeof(Volume));
    }

    public void registerMeasurementUnit<MeasurementUnitType>(string name, MeasurementUnitType measurementUnitType) where MeasurementUnitType: Type
    {
        measurementUnits[name.ToLower()] = measurementUnitType;
    }

    public void unregisterMeasurementUnitFactory(string name)
    {
        measurementUnits.Remove(name.ToLower());
    }

    public MeasurementUnit? CreateMeasurementUnitFrom(string name, string unit, string amount)
    {
        if(measurementUnits.TryGetValue(name.ToLower(), out var factory))
        {
            factory.GetField("Deserialize")?.Invoke(null, new object[] {unit, amount});

            return factory.(unit, amount);
        }

        return null;
    }
}
