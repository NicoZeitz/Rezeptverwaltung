using Core.Interfaces;

namespace Logging;

public class LogLevelFilter : LoggerDecorator
{
    private readonly LogLevel minimumLogLevel;

    public LogLevelFilter(Logger logger, LogLevel minimumLogLevel)
        : base(logger)
    {
        this.minimumLogLevel = minimumLogLevel;
    }

    public override void Log(LogLevel level, string message)
    {
        if (level < minimumLogLevel)
            return;

        logger.Log(level, message);
    }
}