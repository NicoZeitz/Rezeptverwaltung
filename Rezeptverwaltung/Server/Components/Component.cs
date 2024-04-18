namespace Server.Components;

public interface Component
{
    Task<string> RenderAsync();
}