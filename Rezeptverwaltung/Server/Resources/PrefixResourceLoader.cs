namespace Server.ResourceLoader;

public class PrefixResourceLoader : IResourceLoader
{
    private readonly string prefix;
    private readonly IResourceLoader innerResourceLoader;

    public PrefixResourceLoader(string prefix, IResourceLoader innerResourceLoader) : base()
    {
        this.prefix = prefix;
        this.innerResourceLoader = innerResourceLoader;
    }

    public Stream? LoadResource(string resourceName)
    {
        return innerResourceLoader.LoadResource(prefix + "/" + resourceName);
    }
}
