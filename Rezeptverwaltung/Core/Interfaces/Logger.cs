namespace Core.Interfaces;

public enum LogLevel : byte
{
    Trace = 0,
    Debug = 1,
    Info = 2,
    Warning = 3,
    Error = 4,
    Critical = 5
}

public interface Logger
{
    void Log(LogLevel level, string message);

    void Log<T>(LogLevel level, T message)
    {
        if (message is Exception exception)
        {
            Log(level, $"{exception.Message}\n{exception.StackTrace}");
            return;
        }

        Log(level, $"{message}");
    }
    void LogTrace<T>(T message) => Log(LogLevel.Trace, message);
    void LogDebug<T>(T message) => Log(LogLevel.Debug, message);
    void LogInfo<T>(T message) => Log(LogLevel.Info, message);
    void LogWarning<T>(T message) => Log(LogLevel.Warning, message);
    void LogError<T>(T message) => Log(LogLevel.Error, message);
    void LogCritical<T>(T message) => Log(LogLevel.Critical, message);
}