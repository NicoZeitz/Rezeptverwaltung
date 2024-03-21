namespace Server.ResourceLoader;

public interface IResourceLoader
{
    Stream? LoadResource(string resourceName);
}
