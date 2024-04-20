using Core.Interfaces;

namespace Logging;

public class FileLogger : Logger
{
    private readonly Core.ValueObjects.File file;

    public FileLogger(Core.ValueObjects.File file) : base()
    {
        this.file = file;
        this.file.Create();
    }

    public void Log(LogLevel level, string message)
    {
        file.Append(message);
    }
}