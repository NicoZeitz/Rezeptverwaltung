using System.Reflection;

namespace Server.ResourceLoader;

public class EmbeddedResourceLoader : ResourceLoader
{
    public EmbeddedResourceLoader() : base() { }

    public Stream? LoadResource(string resourceName)
    {
        resourceName = $"Server/template/{resourceName}".Replace("/", ".");

        return Assembly.GetExecutingAssembly()!.GetManifestResourceStream(resourceName);
    }
}
