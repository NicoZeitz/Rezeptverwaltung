namespace Server.ResourceLoader;

public class FileSystemResourceLoader : IResourceLoader
{
    // TODO: Directory value object
    public string RootDirectory { get; }

    public FileSystemResourceLoader(string rootDirectory)
    {
        RootDirectory = rootDirectory;
    }

    public Stream? LoadResource(string resourceName)
    {
        Console.WriteLine($"Loading resource: {resourceName}");

        var path = Path.Combine(RootDirectory, resourceName);

        if (!File.Exists(path))
        {
            return null;
        }

        while (true)
        {

            try
            {


                return new FileStream(
                    path,
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
