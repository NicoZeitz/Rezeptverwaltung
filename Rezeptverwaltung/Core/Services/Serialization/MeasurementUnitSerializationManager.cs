﻿using Core.Interfaces;
using Core.ValueObjects.MeasurementUnits;

namespace Core.Services.Serialization;

public class MeasurementUnitSerializationManager
{
    public MeasurementUnitSerializationManager() : base() { }

    public MeasurementUnit? DeserializeFrom(SerializedMeasurementUnit serializedMeasurementUnit)
    {
        serializedMeasurementUnit = serializedMeasurementUnit with
        {
            Unit = serializedMeasurementUnit.Unit.ToLower()
        };

        // TODO: Open-closed principle violation
        switch (serializedMeasurementUnit.Unit)
        {
            case nameof(VolumeUnit.ml):
            case nameof(VolumeUnit.l):
            case nameof(VolumeUnit.kl):
                return new VolumeSerializer().Deserialize(serializedMeasurementUnit);
            case nameof(WeightUnit.g):
            case nameof(WeightUnit.kg):
            case nameof(WeightUnit.t):
                return new WeightSerializer().Deserialize(serializedMeasurementUnit);
            case "x":
                return new PieceSerializer().Deserialize(serializedMeasurementUnit);
            case "prise":
                return new PinchSerializer().Deserialize(serializedMeasurementUnit);
            case "tasse":
                return new CupSerializer().Deserialize(serializedMeasurementUnit);
            case "teelöffel":
            case "dessertlöffel":
            case "löffel":
            case "schöpfkelle":
                return new SpoonSerializer().Deserialize(serializedMeasurementUnit);
            default:
                return null;
        }
    }

    public SerializedMeasurementUnit SerializeInto(MeasurementUnit measurementUnit)
    {
        // TODO: Open-closed principle violation
        switch (measurementUnit)
        {
            case Spoon spoon:
                return new SpoonSerializer().Serialize(spoon);
            case Pinch pinch:
                return new PinchSerializer().Serialize(pinch);
            case Piece piece:
                return new PieceSerializer().Serialize(piece);
            case Volume volume:
                return new VolumeSerializer().Serialize(volume);
            case Weight weight:
                return new WeightSerializer().Serialize(weight);
            case Cup cup:
                return new CupSerializer().Serialize(cup);
            default:
                throw new ArgumentException("Unknown measurement unit");
        }
    }
}
