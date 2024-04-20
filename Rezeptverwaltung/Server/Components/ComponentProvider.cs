namespace Server.Components;

public interface ComponentProvider
{
    T GetComponent<T>() where T : Component;
}