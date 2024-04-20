using Core.Interfaces;

namespace Logging;

public class ConsoleLogger : Logger
{
    public ConsoleLogger() : base() { }

    public void Log(LogLevel level, string message)
    {
        Console.WriteLine(message);
    }
}