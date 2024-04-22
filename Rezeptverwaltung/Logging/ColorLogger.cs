using Core.Interfaces;

namespace Logging;

public class ColorLogger(Logger logger) : LoggerDecorator(logger)
{
    public override void Log(LogLevel level, string message)
    {
        Console.ForegroundColor = level switch
        {
            LogLevel.Trace => ConsoleColor.DarkGray,
            LogLevel.Debug => ConsoleColor.Gray,
            LogLevel.Info => ConsoleColor.White,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Critical => ConsoleColor.DarkRed,
            _ => Console.ForegroundColor
        };

        logger.Log(level, message);

        Console.ResetColor();
    }
}