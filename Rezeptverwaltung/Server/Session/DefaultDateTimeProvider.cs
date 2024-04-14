namespace Server.Session;

public class DefaultDateTimeProvider : DateTimeProvider
{
    public DateTime Now => DateTime.Now;

    public DateTime UtcNow => DateTime.UtcNow;

    public DateTimeOffset OffsetNow => DateTimeOffset.Now;

    public DateTimeOffset OffsetUtcNow => DateTime.UtcNow;
}
