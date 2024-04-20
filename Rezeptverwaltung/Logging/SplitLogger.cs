using Core.Interfaces;

namespace Logging;

public class SplitLogger : LoggerDecorator
{
    private readonly Logger leftLogger;
    private readonly Logger rightLogger;

    public SplitLogger(Logger leftLogger, Logger rightLogger) : base(leftLogger)
    {
        this.leftLogger = leftLogger;
        this.rightLogger = rightLogger;
    }

    public override void Log(LogLevel level, string message)
    {
        leftLogger.Log(level, message);
        rightLogger.Log(level, message);
    }
}