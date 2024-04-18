using Core.Interfaces;

namespace Server.Components;

public class DisplayableComponent : Component
{
    private readonly Displayable displayable;

    public DisplayableComponent(Displayable displayable) : base()
    {
        this.displayable = displayable;
    }

    public Task<string> RenderAsync()
    {
        return Task.FromResult(displayable.display());
    }
}