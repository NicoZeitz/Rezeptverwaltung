namespace Core.ValueObjects;

public record struct Duration(TimeSpan TimeSpan) : IComparable<Duration>
{
    public int CompareTo(Duration other) => TimeSpan.CompareTo(other.TimeSpan);

    public override string ToString()
    {
        // TODO: write tests for formatting
        if (TimeSpan.TotalMinutes < 1)
            return $"{TimeSpan.TotalSeconds:.2} Sekunden";

        if (TimeSpan.TotalHours < 1 && TimeSpan.Seconds == 0)
            return $"{TimeSpan.Minutes} Minuten";

        if (TimeSpan.TotalHours < 1)
            return $"{TimeSpan:mm}:{TimeSpan:ss}";

        if (TimeSpan.TotalDays < 1 && TimeSpan.Minutes == 0 && TimeSpan.Seconds == 0)
            return string.Format(@"{0:hh} Stunden", TimeSpan);

        return string.Format(@"{0:hh}:{0:mm}:{0:ss}", TimeSpan);
    }
}