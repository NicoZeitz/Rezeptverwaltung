using Core.Interfaces;

namespace Logging;

public class DatetimeLogger : LoggerDecorator
{
    private readonly DateTimeProvider dateTimeProvider;

    public DatetimeLogger(Logger logger, DateTimeProvider dateTimeProvider) : base(logger)
    {
        this.dateTimeProvider = dateTimeProvider;
    }

    public override void Log(LogLevel level, string message)
    {
        logger.Log(level, $"[{dateTimeProvider.Now:yyyy-MM-dd HH:mm:ss}] {message}");
    }
}