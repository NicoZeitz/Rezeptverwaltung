using Core.Interfaces;

namespace Logging;

public abstract class LoggerDecorator : Logger
{
    protected readonly Logger logger;

    protected LoggerDecorator(Logger logger)
    {
        this.logger = logger;
    }

    public abstract void Log(LogLevel level, string message);
}