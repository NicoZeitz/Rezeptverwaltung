namespace Server.Session;

public interface DateTimeProvider
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    DateTimeOffset OffsetNow { get; }
    DateTimeOffset OffsetUtcNow { get; }
}
