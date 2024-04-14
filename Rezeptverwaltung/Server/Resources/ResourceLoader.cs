namespace Server.ResourceLoader;

public interface ResourceLoader
{
    Stream? LoadResource(string resourceName);
}
