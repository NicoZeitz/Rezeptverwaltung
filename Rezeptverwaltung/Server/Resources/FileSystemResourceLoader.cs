using Core.Interfaces;
using Core.ValueObjects;

namespace Server.ResourceLoader;

public class FileSystemResourceLoader : ResourceLoader
{
    private readonly Core.ValueObjects.Directory rootDirectory;
    private readonly Logger logger;

    public FileSystemResourceLoader(Core.ValueObjects.Directory rootDirectory, Logger logger)
    {
        this.logger = logger;
        this.rootDirectory = rootDirectory;
    }

    public Stream? LoadResource(string resourceName)
    {
        logger.LogInfo($"Loading resource: {resourceName}");

        var path = new Core.ValueObjects.File(rootDirectory, FileName.From(resourceName));
        if (!path.Exists())
            return null;

        while (true)
        {
            try
            {
                return new FileStream(
                    path.FullName,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 4096,
                    useAsync: true
                );
            }
            catch
            {
                Thread.Sleep(100);
            }
        }
    }

}
