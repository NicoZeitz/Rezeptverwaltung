namespace Core.Interfaces;

public interface MeasurementUnit : Displayable
{
    public string DisplayUnit { get; }
    public string DisplayAmount { get; }
}