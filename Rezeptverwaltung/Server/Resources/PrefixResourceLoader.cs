namespace Server.ResourceLoader;

public class PrefixResourceLoader : ResourceLoader
{
    private readonly string prefix;
    private readonly ResourceLoader innerResourceLoader;

    public PrefixResourceLoader(string prefix, ResourceLoader innerResourceLoader) : base()
    {
        this.prefix = prefix;
        this.innerResourceLoader = innerResourceLoader;
    }

    public Stream? LoadResource(string resourceName)
    {
        return innerResourceLoader.LoadResource(prefix + "/" + resourceName);
    }
}
