using Core.Interfaces;

namespace Logging;

public class LogLevelLogger(Logger logger) : LoggerDecorator(logger)
{
    public override void Log(LogLevel level, string message)
    {
        logger.Log(level, $"[{level}] {message}");
    }
}

