namespace Core.ValueObjects;

public record struct Duration(TimeSpan TimeSpan) : IComparable<Duration>
{
    public readonly int CompareTo(Duration other) => TimeSpan.CompareTo(other.TimeSpan);

    public override readonly string ToString()
    {
        if (TimeSpan.TotalMinutes < 1)
            return $"{TimeSpan.TotalSeconds:.2} Sekunden";

        if (TimeSpan.TotalHours < 1 && TimeSpan.Seconds == 0)
            return $"{TimeSpan.Minutes} Minuten";

        if (TimeSpan.TotalHours < 1)
            return $"{TimeSpan:mm}:{TimeSpan:ss}";

        if (TimeSpan.TotalDays < 1 && TimeSpan.Minutes == 0 && TimeSpan.Seconds == 0)
            return string.Format(@"{0:hh} Stunden", TimeSpan);

        if (TimeSpan.TotalDays < 1)
            return string.Format(@"{0:hh}:{0:mm}", TimeSpan);

        return string.Format(@"{0:%d} Tage {0:hh}:{0:mm}:{0:ss}", TimeSpan);
    }

    public static Duration? Parse(string durationString)
    {
        // HH:mm or mm or HH:mm:ss
        var parts = durationString.Split(':');
        if (parts.Length == 1)
        {
            return new Duration(TimeSpan.FromMinutes(int.Parse(parts[0])));
        }
        if (parts.Length == 2)
        {
            return new Duration(TimeSpan.FromMinutes(int.Parse(parts[0]) * 60 + int.Parse(parts[1])));
        }
        if (parts.Length == 3)
        {
            return new Duration(TimeSpan.FromMinutes(int.Parse(parts[0]) * 60 + int.Parse(parts[1]) + int.Parse(parts[2])));
        }
        return null;
    }
}