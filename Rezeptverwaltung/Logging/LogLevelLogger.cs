using Core.Interfaces;

namespace Logging;

public class LogLevelLogger : LoggerDecorator
{
    public LogLevelLogger(Logger logger) : base(logger) { }

    public override void Log(LogLevel level, string message)
    {
        logger.Log(level, $"[{level}] {message}");
    }
}

