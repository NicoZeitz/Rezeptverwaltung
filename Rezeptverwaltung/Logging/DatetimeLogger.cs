using Core.Interfaces;

namespace Logging;

public class DateTimeLogger : LoggerDecorator
{
    private readonly DateTimeProvider dateTimeProvider;

    public DateTimeLogger(Logger logger, DateTimeProvider dateTimeProvider) : base(logger)
    {
        this.dateTimeProvider = dateTimeProvider;
    }

    public override void Log(LogLevel level, string message)
    {
        logger.Log(level, $"[{dateTimeProvider.Now:yyyy-MM-dd HH:mm:ss}] {message}");
    }
}